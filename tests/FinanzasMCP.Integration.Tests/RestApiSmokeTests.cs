using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Budgets;
using FinanzasMCP.Domain.Categories;
using FinanzasMCP.Domain.Goals;
using FinanzasMCP.Domain.Shopping;
using FinanzasMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FinanzasMCP.Integration.Tests;

[CollectionDefinition("api", DisableParallelization = true)]
public sealed class ApiCollectionDefinition;

[Collection("api")]
public sealed class RestApiSmokeTests(ApiTestFactory factory) : IClassFixture<ApiTestFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public async Task Health_endpoint_returns_ok()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OpenAI_apps_challenge_endpoint_returns_configured_token()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/.well-known/openai-apps-challenge");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("test-openai-apps-challenge", body);
    }

    [Fact]
    public async Task Cors_allows_any_origin()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/health");
        request.Headers.Add("Origin", "http://localhost:4200");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins));
        Assert.Contains("http://localhost:4200", origins);
    }

    [Fact]
    public async Task Accounts_and_user_context_flow_works()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var createAccountResponse = await client.PostAsJsonAsync("/api/v1/accounts", new
        {
            name = "Cuenta REST",
            accountType = AccountType.Bank,
            currency = "USD",
            balance = 1250m,
            bankName = "Banco Demo",
            accountNumber = "00112233",
            provider = "Banco Demo",
            cryptoSymbol = (string?)null,
            cryptoNetwork = (string?)null,
            cryptoQuantity = (decimal?)null,
            cryptoAvgBuyPriceUsd = (decimal?)null
        });

        createAccountResponse.EnsureSuccessStatusCode();
        var account = await createAccountResponse.Content.ReadFromJsonAsync<AccountSummary>(JsonOptions);
        Assert.NotNull(account);
        Assert.Equal("Cuenta REST", account!.Name);

        var accountsResponse = await client.GetAsync("/api/v1/accounts");
        accountsResponse.EnsureSuccessStatusCode();
        var accounts = await accountsResponse.Content.ReadFromJsonAsync<AccountSummary[]>(JsonOptions);
        Assert.NotNull(accounts);
        Assert.Contains(accounts!, x => x.Id == account.Id);

        var userContextResponse = await client.PutAsJsonAsync("/api/v1/user-context/project", new { value = "finanzas" });
        userContextResponse.EnsureSuccessStatusCode();

        var userContextList = await client.GetFromJsonAsync<UserContextEntrySummary[]>("/api/v1/user-context", JsonOptions);
        Assert.NotNull(userContextList);
        Assert.Contains(userContextList!, x => x.Key == "project" && x.Value == "finanzas");
    }

    [Fact]
    public async Task Negative_transaction_is_rejected()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var createAccountResponse = await client.PostAsJsonAsync("/api/v1/accounts", new
        {
            name = "Cuenta TX",
            accountType = AccountType.Bank,
            currency = "USD",
            balance = 100m,
            bankName = (string?)null,
            accountNumber = (string?)null,
            provider = (string?)null,
            cryptoSymbol = (string?)null,
            cryptoNetwork = (string?)null,
            cryptoQuantity = (decimal?)null,
            cryptoAvgBuyPriceUsd = (decimal?)null
        });

        createAccountResponse.EnsureSuccessStatusCode();
        var account = await createAccountResponse.Content.ReadFromJsonAsync<AccountSummary>(JsonOptions);
        Assert.NotNull(account);
        var category = await CreateCategoryAsync(client, "Transacciones invalidas");

        var response = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Expense",
            amount = -1m,
            currency = "USD",
            accountId = account!.Id,
            toAccountId = (Guid?)null,
            categoryId = category.Id,
            description = "Invalid",
            reference = (string?)null,
            transactionDate = DateTimeOffset.UtcNow,
            recurringRuleId = (Guid?)null,
            tagIds = Array.Empty<Guid>()
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Expense_requires_category_but_income_does_not()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        await AuthenticateAsync(client);
        var account = await CreateAccountAsync(client, "Cuenta categorias requeridas");

        var expenseWithoutCategory = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Expense",
            amount = 10m,
            currency = "USD",
            accountId = account.Id,
            toAccountId = (Guid?)null,
            categoryId = (Guid?)null,
            budgetId = (Guid?)null,
            description = "Gasto sin categoria",
            reference = (string?)null,
            transactionDate = DateTimeOffset.UtcNow,
            recurringRuleId = (Guid?)null,
            tagIds = Array.Empty<Guid>()
        });
        Assert.Equal(HttpStatusCode.BadRequest, expenseWithoutCategory.StatusCode);

        var incomeWithoutCategory = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Income",
            amount = 10m,
            currency = "USD",
            accountId = account.Id,
            toAccountId = (Guid?)null,
            categoryId = (Guid?)null,
            budgetId = (Guid?)null,
            description = "Ingreso sin categoria",
            reference = (string?)null,
            transactionDate = DateTimeOffset.UtcNow,
            recurringRuleId = (Guid?)null,
            tagIds = Array.Empty<Guid>()
        });
        Assert.True(incomeWithoutCategory.IsSuccessStatusCode, await incomeWithoutCategory.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Account_can_be_deleted()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var createAccountResponse = await client.PostAsJsonAsync("/api/v1/accounts", new
        {
            name = "Cuenta a borrar",
            accountType = AccountType.Cash,
            currency = "USD",
            balance = 0m,
            bankName = (string?)null,
            accountNumber = (string?)null,
            provider = (string?)null,
            cryptoSymbol = (string?)null,
            cryptoNetwork = (string?)null,
            cryptoQuantity = (decimal?)null,
            cryptoAvgBuyPriceUsd = (decimal?)null
        });

        createAccountResponse.EnsureSuccessStatusCode();
        var account = await createAccountResponse.Content.ReadFromJsonAsync<AccountSummary>(JsonOptions);
        Assert.NotNull(account);

        var deleteResponse = await client.DeleteAsync($"/api/v1/accounts/{account!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Financial_goal_flow_works_through_new_rest_api()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        await AuthenticateAsync(client);
        var sourceAccount = await CreateAccountAsync(client, "Cuenta objetivos");

        var createResponse = await client.PostAsJsonAsync("/api/v1/goals", new
        {
            name = "Fondo de emergencia",
            targetAmount = 1000m,
            type = FinancialGoalType.Saving,
            description = "Reserva",
            priority = 1,
            url = (string?)null,
            accountId = (Guid?)null,
            targetDate = DateTimeOffset.UtcNow.AddMonths(6)
        });
        var createBody = await createResponse.Content.ReadAsStringAsync();
        Assert.True(createResponse.IsSuccessStatusCode, createBody);
        var goal = JsonSerializer.Deserialize<FinancialGoalTestResponse>(createBody, JsonOptions);
        Assert.NotNull(goal);
        Assert.Equal(FinancialGoalType.Saving, goal!.Type);

        var contributionResponse = await client.PostAsJsonAsync($"/api/v1/goals/{goal.Id}/contributions", new
        {
            amount = 125m,
            contributionDate = DateTimeOffset.UtcNow,
            transactionId = (Guid?)null,
            accountId = sourceAccount.Id
        });
        var contributionBody = await contributionResponse.Content.ReadAsStringAsync();
        Assert.True(contributionResponse.IsSuccessStatusCode, contributionBody);
        var contributed = JsonSerializer.Deserialize<FinancialGoalTestResponse>(contributionBody, JsonOptions);
        Assert.NotNull(contributed);
        Assert.Equal(125m, contributed!.CurrentAmount);

        var contributions = await client.GetFromJsonAsync<FinancialGoalContributionTestResponse[]>($"/api/v1/goal-contributions?goalId={goal.Id}", JsonOptions);
        Assert.NotNull(contributions);
        Assert.Single(contributions!);
        Assert.Equal(125m, contributions[0].Amount);
    }

    [Fact]
    public async Task Updating_migrated_goal_contribution_creates_missing_transaction()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        var auth = await AuthenticateAsync(client);
        var sourceAccount = await CreateAccountAsync(client, "Cuenta aporte migrado");

        var createResponse = await client.PostAsJsonAsync("/api/v1/goals", new
        {
            name = "Meta migrada",
            targetAmount = 500m,
            type = FinancialGoalType.Saving,
            description = (string?)null,
            priority = 1,
            url = (string?)null,
            accountId = (Guid?)null,
            targetDate = (DateTimeOffset?)null
        });
        var createBody = await createResponse.Content.ReadAsStringAsync();
        Assert.True(createResponse.IsSuccessStatusCode, createBody);
        var goal = JsonSerializer.Deserialize<FinancialGoalTestResponse>(createBody, JsonOptions);
        Assert.NotNull(goal);

        var contributionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanzasMCPDbContext>();
            await dbContext.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO financial_goal_contributions
                    ("Id", "GoalId", "TransactionId", "Amount", "ContributionDate", "CreatedAt", "UpdatedAt", "DeletedAt", "UserId")
                VALUES
                    ({contributionId}, {goal!.Id}, NULL, {25m}, {now}, {now}, {now}, NULL, {auth.User.Id})
                """);
        }

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/goal-contributions/{contributionId}", new
        {
            amount = 40m,
            contributionDate = now.AddDays(1),
            transactionId = (Guid?)null,
            accountId = sourceAccount.Id
        });
        var updateBody = await updateResponse.Content.ReadAsStringAsync();
        Assert.True(updateResponse.IsSuccessStatusCode, updateBody);

        var contributions = await client.GetFromJsonAsync<FinancialGoalContributionTestResponse[]>($"/api/v1/goal-contributions?goalId={goal.Id}", JsonOptions);
        Assert.NotNull(contributions);
        var updatedContribution = Assert.Single(contributions!);
        Assert.NotNull(updatedContribution.TransactionId);
        Assert.Equal(sourceAccount.Id, updatedContribution.AccountId);
        Assert.Equal(40m, updatedContribution.Amount);

        var accounts = await client.GetFromJsonAsync<AccountSummary[]>("/api/v1/accounts", JsonOptions);
        Assert.NotNull(accounts);
        Assert.Equal(210m, accounts!.Single(x => x.Id == sourceAccount.Id).Balance);
    }

    [Fact]
    public async Task Legacy_goal_routes_use_unified_model()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var savingResponse = await client.PostAsJsonAsync("/api/v1/saving-goals", new
        {
            name = "Viaje",
            targetAmount = 800m,
            accountId = (Guid?)null,
            targetDate = (DateTimeOffset?)null
        });
        var savingBody = await savingResponse.Content.ReadAsStringAsync();
        Assert.True(savingResponse.IsSuccessStatusCode, savingBody);
        var saving = JsonSerializer.Deserialize<FinancialGoalTestResponse>(savingBody, JsonOptions);
        Assert.NotNull(saving);
        Assert.Equal(FinancialGoalType.Saving, saving!.Type);

        var purchaseResponse = await client.PostAsJsonAsync("/api/v1/purchase-goals", new
        {
            name = "Laptop",
            targetPrice = 1500m,
            description = "Trabajo",
            priority = 2,
            url = "https://example.com/laptop",
            accountId = (Guid?)null,
            targetDate = (DateTimeOffset?)null
        });
        var purchaseBody = await purchaseResponse.Content.ReadAsStringAsync();
        Assert.True(purchaseResponse.IsSuccessStatusCode, purchaseBody);
        var purchase = JsonSerializer.Deserialize<FinancialGoalTestResponse>(purchaseBody, JsonOptions);
        Assert.NotNull(purchase);
        Assert.Equal(FinancialGoalType.Purchase, purchase!.Type);
        Assert.Equal("https://example.com/laptop", purchase.Url);

        var goals = await client.GetFromJsonAsync<FinancialGoalTestResponse[]>("/api/v1/goals", JsonOptions);
        Assert.NotNull(goals);
        Assert.Contains(goals!, x => x.Id == saving.Id);
        Assert.Contains(goals!, x => x.Id == purchase.Id);
    }

    [Fact]
    public async Task Transaction_supports_attachment_upload_and_listing()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var createAccountResponse = await client.PostAsJsonAsync("/api/v1/accounts", new
        {
            name = "Cuenta evidencia",
            accountType = AccountType.Bank,
            currency = "USD",
            balance = 500m,
            bankName = (string?)null,
            accountNumber = (string?)null,
            provider = (string?)null,
            cryptoSymbol = (string?)null,
            cryptoNetwork = (string?)null,
            cryptoQuantity = (decimal?)null,
            cryptoAvgBuyPriceUsd = (decimal?)null
        });

        createAccountResponse.EnsureSuccessStatusCode();
        var account = await createAccountResponse.Content.ReadFromJsonAsync<AccountSummary>(JsonOptions);
        Assert.NotNull(account);
        var category = await CreateCategoryAsync(client, "Taxi");

        var createTransactionResponse = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Expense",
            amount = 25m,
            currency = "USD",
            accountId = account!.Id,
            toAccountId = (Guid?)null,
            categoryId = category.Id,
            budgetId = (Guid?)null,
            description = "Taxi",
            reference = (string?)null,
            transactionDate = DateTimeOffset.UtcNow,
            recurringRuleId = (Guid?)null,
            tagIds = Array.Empty<Guid>()
        });

        createTransactionResponse.EnsureSuccessStatusCode();
        var transaction = await createTransactionResponse.Content.ReadFromJsonAsync<TransactionSummary>(JsonOptions);
        Assert.NotNull(transaction);

        using var multipart = new MultipartFormDataContent();
        var image = new ByteArrayContent("fake-image-content"u8.ToArray());
        image.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        multipart.Add(image, "files", "evidencia.png");

        var uploadResponse = await client.PostAsync($"/api/v1/transactions/{transaction!.Id}/attachments", multipart);
        uploadResponse.EnsureSuccessStatusCode();
        var attachments = await uploadResponse.Content.ReadFromJsonAsync<TransactionAttachmentSummary[]>(JsonOptions);

        Assert.NotNull(attachments);
        Assert.Single(attachments!);
        Assert.Equal("evidencia.png", attachments[0].FileName);

        var listHttpResponse = await client.GetAsync($"/api/v1/transactions/{transaction.Id}/attachments");
        var listBody = await listHttpResponse.Content.ReadAsStringAsync();
        Assert.True(listHttpResponse.IsSuccessStatusCode, listBody);
        var listResponse = JsonSerializer.Deserialize<TransactionAttachmentSummary[]>(listBody, JsonOptions);
        Assert.NotNull(listResponse);
        Assert.Single(listResponse!);
    }

    [Fact]
    public async Task Budget_usage_history_and_transaction_filter_are_calculated_by_api()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        await AuthenticateAsync(client);
        var account = await CreateAccountAsync(client, "Cuenta presupuesto");
        var category = await CreateCategoryAsync(client, "Comida");

        var budgetResponse = await client.PostAsJsonAsync("/api/v1/budgets", new
        {
            name = "Comida",
            limitAmount = 50m,
            periodType = PeriodType.Monthly,
            validityType = BudgetValidityType.Indefinite,
            periodStart = (DateTimeOffset?)null,
            periodEnd = (DateTimeOffset?)null
        });
        var budgetBody = await budgetResponse.Content.ReadAsStringAsync();
        Assert.True(budgetResponse.IsSuccessStatusCode, budgetBody);
        var budget = JsonSerializer.Deserialize<BudgetTestResponse>(budgetBody, JsonOptions);
        Assert.NotNull(budget);

        var now = DateTimeOffset.UtcNow;
        var firstExpense = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Expense",
            amount = 30m,
            currency = "USD",
            accountId = account.Id,
            toAccountId = (Guid?)null,
            categoryId = category.Id,
            budgetId = budget!.Id,
            description = "Almuerzo",
            reference = (string?)null,
            transactionDate = now,
            recurringRuleId = (Guid?)null,
            tagIds = Array.Empty<Guid>()
        });
        Assert.True(firstExpense.IsSuccessStatusCode, await firstExpense.Content.ReadAsStringAsync());

        var secondExpense = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Expense",
            amount = 25m,
            currency = "USD",
            accountId = account.Id,
            toAccountId = (Guid?)null,
            categoryId = category.Id,
            budgetId = budget.Id,
            description = "Cena",
            reference = (string?)null,
            transactionDate = now,
            recurringRuleId = (Guid?)null,
            tagIds = Array.Empty<Guid>()
        });
        Assert.True(secondExpense.IsSuccessStatusCode, await secondExpense.Content.ReadAsStringAsync());

        var incomeWithBudget = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Income",
            amount = 10m,
            currency = "USD",
            accountId = account.Id,
            toAccountId = (Guid?)null,
            categoryId = (Guid?)null,
            budgetId = budget.Id,
            description = "Ingreso invalido",
            reference = (string?)null,
            transactionDate = now,
            recurringRuleId = (Guid?)null,
            tagIds = Array.Empty<Guid>()
        });
        Assert.Equal(HttpStatusCode.BadRequest, incomeWithBudget.StatusCode);

        var budgetsResponse = await client.GetAsync("/api/v1/budgets");
        var budgetsBody = await budgetsResponse.Content.ReadAsStringAsync();
        Assert.True(budgetsResponse.IsSuccessStatusCode, budgetsBody);
        var budgets = JsonSerializer.Deserialize<BudgetTestResponse[]>(budgetsBody, JsonOptions);
        Assert.NotNull(budgets);
        var updatedBudget = Assert.Single(budgets!, x => x.Id == budget.Id);
        Assert.Equal(55m, updatedBudget.UsedAmount);
        Assert.True(updatedBudget.IsOverLimit);
        Assert.Equal(2, updatedBudget.TransactionCount);

        var filteredTransactions = await client.GetFromJsonAsync<PagedResult<TransactionSummary>>($"/api/v1/transactions?budgetId={budget.Id}&page=1&pageSize=10", JsonOptions);
        Assert.NotNull(filteredTransactions);
        Assert.Equal(2, filteredTransactions!.TotalCount);

        var dateFrom = Uri.EscapeDataString(now.AddDays(-1).ToString("O"));
        var dateTo = Uri.EscapeDataString(now.AddDays(1).ToString("O"));
        var historyResponse = await client.GetAsync($"/api/v1/budgets/{budget.Id}/usage-history?groupBy=month&dateFrom={dateFrom}&dateTo={dateTo}");
        var historyBody = await historyResponse.Content.ReadAsStringAsync();
        Assert.True(historyResponse.IsSuccessStatusCode, historyBody);
        var history = JsonSerializer.Deserialize<BudgetHistoryTestResponse[]>(historyBody, JsonOptions);
        Assert.NotNull(history);
        Assert.Contains(history!, point => point.SpentAmount == 55m && point.IsOverLimit);
    }

    [Fact]
    public async Task Large_image_attachment_is_optimized_before_storage()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var createAccountResponse = await client.PostAsJsonAsync("/api/v1/accounts", new
        {
            name = "Cuenta optimizacion",
            accountType = AccountType.Bank,
            currency = "USD",
            balance = 500m,
            bankName = (string?)null,
            accountNumber = (string?)null,
            provider = (string?)null,
            cryptoSymbol = (string?)null,
            cryptoNetwork = (string?)null,
            cryptoQuantity = (decimal?)null,
            cryptoAvgBuyPriceUsd = (decimal?)null
        });

        createAccountResponse.EnsureSuccessStatusCode();
        var account = await createAccountResponse.Content.ReadFromJsonAsync<AccountSummary>(JsonOptions);
        Assert.NotNull(account);
        var category = await CreateCategoryAsync(client, "Evidencias");

        var createTransactionResponse = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Expense",
            amount = 42m,
            currency = "USD",
            accountId = account!.Id,
            toAccountId = (Guid?)null,
            categoryId = category.Id,
            budgetId = (Guid?)null,
            description = "Compra con evidencia",
            reference = (string?)null,
            transactionDate = DateTimeOffset.UtcNow,
            recurringRuleId = (Guid?)null,
            tagIds = Array.Empty<Guid>()
        });

        createTransactionResponse.EnsureSuccessStatusCode();
        var transaction = await createTransactionResponse.Content.ReadFromJsonAsync<TransactionSummary>(JsonOptions);
        Assert.NotNull(transaction);

        var originalImage = await CreateDetailedJpegAsync(1800, 1200);
        using var multipart = new MultipartFormDataContent();
        var image = new ByteArrayContent(originalImage);
        image.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        multipart.Add(image, "files", "recibo.jpg");

        var uploadResponse = await client.PostAsync($"/api/v1/transactions/{transaction!.Id}/attachments", multipart);
        var uploadBody = await uploadResponse.Content.ReadAsStringAsync();
        Assert.True(uploadResponse.IsSuccessStatusCode, uploadBody);
        var attachments = await uploadResponse.Content.ReadFromJsonAsync<TransactionAttachmentSummary[]>(JsonOptions);

        Assert.NotNull(attachments);
        Assert.Single(attachments!);
        Assert.Equal("image/jpeg", attachments[0].ContentType);
        Assert.EndsWith(".jpg", attachments[0].FileName, StringComparison.OrdinalIgnoreCase);
        Assert.True(attachments[0].SizeBytes < originalImage.Length, $"Expected optimized size to be smaller than original. Original={originalImage.Length}, Optimized={attachments[0].SizeBytes}");

        var contentResponse = await client.GetAsync($"/api/v1/transaction-attachments/{attachments[0].Id}/content");
        contentResponse.EnsureSuccessStatusCode();
        Assert.Equal("image/jpeg", contentResponse.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Financial_endpoints_require_authentication()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/accounts");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Api_key_can_access_only_own_user_data()
    {
        factory.InitializeDatabase();

        var ownerClient = factory.CreateClient();
        await AuthenticateAsync(ownerClient);
        var ownerAccount = await CreateAccountAsync(ownerClient, "Cuenta API Owner");
        var apiKey = await CreateApiKeyAsync(ownerClient, "Claude Desktop");

        var otherClient = factory.CreateClient();
        await AuthenticateAsync(otherClient);
        var otherAccount = await CreateAccountAsync(otherClient, "Cuenta API Other");

        var apiClient = factory.CreateClient();
        apiClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

        var response = await apiClient.GetAsync("/api/v1/accounts");
        response.EnsureSuccessStatusCode();
        var accounts = await response.Content.ReadFromJsonAsync<AccountSummary[]>(JsonOptions);

        Assert.NotNull(accounts);
        Assert.Contains(accounts!, x => x.Id == ownerAccount.Id);
        Assert.DoesNotContain(accounts!, x => x.Id == otherAccount.Id);
    }

    [Fact]
    public async Task Shopping_crud_and_recommendation_flow_works()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        await AuthenticateAsync(client);

        var tutti = await CreateShoppingStoreAsync(client, "Tutti");
        var supermaxi = await CreateShoppingStoreAsync(client, "Supermaxi");
        var product = await CreateShoppingProductAsync(client, "Huevos");
        var variant = await CreateShoppingVariantAsync(client, product.Id, "Huevos grandes x12", 12m, ShoppingUnit.Unit);

        await client.PostAsJsonAsync("/api/v1/shopping/prices", new
        {
            storeId = tutti.Id,
            productVariantId = variant.Id,
            totalPrice = 2.40m,
            normalizedQuantity = 12m,
            unit = ShoppingUnit.Unit,
            observedAt = DateTimeOffset.UtcNow,
            notes = (string?)null
        });
        await client.PostAsJsonAsync("/api/v1/shopping/prices", new
        {
            storeId = supermaxi.Id,
            productVariantId = variant.Id,
            totalPrice = 3.00m,
            normalizedQuantity = 12m,
            unit = ShoppingUnit.Unit,
            observedAt = DateTimeOffset.UtcNow,
            notes = (string?)null
        });

        var listResponse = await client.PostAsJsonAsync("/api/v1/shopping/lists", new
        {
            name = "Lista prueba",
            listDate = DateTimeOffset.UtcNow,
            transactionId = (Guid?)null,
            items = new[]
            {
                new { productVariantId = variant.Id, desiredQuantity = 12m, unit = ShoppingUnit.Unit, notes = (string?)null }
            }
        });
        var listBody = await listResponse.Content.ReadAsStringAsync();
        Assert.True(listResponse.IsSuccessStatusCode, listBody);
        var list = JsonSerializer.Deserialize<ShoppingListTestResponse>(listBody, JsonOptions);
        Assert.NotNull(list);

        var recommendation = await client.GetFromJsonAsync<ShoppingRecommendationTestResponse>($"/api/v1/shopping/lists/{list!.Id}/recommendation", JsonOptions);

        Assert.NotNull(recommendation);
        var group = Assert.Single(recommendation!.StoreGroups);
        Assert.Equal(tutti.Id, group.StoreId);
        Assert.Equal(2.40m, group.Subtotal);
        Assert.Equal(0.60m, recommendation.EstimatedSavings);
    }

    [Fact]
    public async Task Shopping_data_is_scoped_to_authenticated_user()
    {
        factory.InitializeDatabase();

        var ownerClient = factory.CreateClient();
        await AuthenticateAsync(ownerClient);
        var ownerStore = await CreateShoppingStoreAsync(ownerClient, "Owner Store");

        var otherClient = factory.CreateClient();
        await AuthenticateAsync(otherClient);
        await CreateShoppingStoreAsync(otherClient, "Other Store");

        var ownerStores = await ownerClient.GetFromJsonAsync<ShoppingStoreTestResponse[]>("/api/v1/shopping/stores", JsonOptions);

        Assert.NotNull(ownerStores);
        Assert.Contains(ownerStores!, x => x.Id == ownerStore.Id);
        Assert.DoesNotContain(ownerStores!, x => x.Name == "Other Store");
    }

    [Fact]
    public async Task Mcp_accepts_x_api_key_header()
    {
        factory.InitializeDatabase();

        var ownerClient = factory.CreateClient();
        await AuthenticateAsync(ownerClient);
        var ownerAccount = await CreateAccountAsync(ownerClient, "Cuenta MCP API Key");
        var apiKey = await CreateApiKeyAsync(ownerClient, "Servidor MCP");

        var mcpClient = factory.CreateClient();
        mcpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        mcpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        mcpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        var response = await mcpClient.PostAsJsonAsync("/mcp", new
        {
            jsonrpc = "2.0",
            id = 1,
            method = "tools/call",
            @params = new
            {
                name = "list_accounts",
                arguments = new { }
            }
        });
        var body = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, body);
        Assert.Empty(response.Headers.WwwAuthenticate);
        Assert.Contains(ownerAccount.Id.ToString(), body);
        Assert.Contains(ownerAccount.Name, body);
    }

    [Fact]
    public async Task Revoked_api_key_is_rejected()
    {
        factory.InitializeDatabase();

        var ownerClient = factory.CreateClient();
        await AuthenticateAsync(ownerClient);
        await CreateAccountAsync(ownerClient, "Cuenta API");
        var apiKeyId = await CreateApiKeyIdAsync(ownerClient, "Servidor MCP");
        var plainTextApiKey = apiKeyId.PlainTextKey;

        var apiClient = factory.CreateClient();
        apiClient.DefaultRequestHeaders.Add("X-API-Key", plainTextApiKey);
        var successResponse = await apiClient.GetAsync("/api/v1/accounts");
        successResponse.EnsureSuccessStatusCode();

        var revokeResponse = await ownerClient.PostAsync($"/api/v1/auth/api-keys/{apiKeyId.Id}/revoke", JsonContent.Create(new { }));
        Assert.Equal(HttpStatusCode.NoContent, revokeResponse.StatusCode);

        var rejectedResponse = await apiClient.GetAsync("/api/v1/accounts");
        Assert.Equal(HttpStatusCode.Unauthorized, rejectedResponse.StatusCode);
    }

    private static async Task<AuthTestResponse> AuthenticateAsync(HttpClient client)
    {
        var email = $"test-{Guid.NewGuid():N}@finanzas.local";
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email,
            password = "Password123!",
            displayName = "Test User"
        });

        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, body);
        var auth = await response.Content.ReadFromJsonAsync<AuthTestResponse>(JsonOptions);
        Assert.NotNull(auth);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
        return auth;
    }

    private async Task<AccountSummary> CreateAccountAsync(HttpClient client, string name)
    {
        var createAccountResponse = await client.PostAsJsonAsync("/api/v1/accounts", new
        {
            name,
            accountType = AccountType.Bank,
            currency = "USD",
            balance = 250m,
            bankName = (string?)null,
            accountNumber = (string?)null,
            provider = (string?)null,
            cryptoSymbol = (string?)null,
            cryptoNetwork = (string?)null,
            cryptoQuantity = (decimal?)null,
            cryptoAvgBuyPriceUsd = (decimal?)null
        });

        createAccountResponse.EnsureSuccessStatusCode();
        var account = await createAccountResponse.Content.ReadFromJsonAsync<AccountSummary>(JsonOptions);
        Assert.NotNull(account);
        return account!;
    }

    private async Task<string> CreateApiKeyAsync(HttpClient client, string name)
    {
        var created = await CreateApiKeyIdAsync(client, name);
        return created.PlainTextKey;
    }

    private async Task<ShoppingStoreTestResponse> CreateShoppingStoreAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/api/v1/shopping/stores", new { name, notes = (string?)null });
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, body);
        var store = JsonSerializer.Deserialize<ShoppingStoreTestResponse>(body, JsonOptions);
        Assert.NotNull(store);
        return store!;
    }

    private async Task<CategorySummary> CreateCategoryAsync(HttpClient client, string name, CategoryType type = CategoryType.Expense)
    {
        var response = await client.PostAsJsonAsync("/api/v1/categories", new
        {
            name,
            type,
            icon = (string?)null,
            parentId = (Guid?)null,
            isSystem = false
        });
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, body);
        var category = JsonSerializer.Deserialize<CategorySummary>(body, JsonOptions);
        Assert.NotNull(category);
        return category!;
    }

    private async Task<ShoppingProductTestResponse> CreateShoppingProductAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/api/v1/shopping/products", new { name, categoryId = (Guid?)null, notes = (string?)null });
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, body);
        var product = JsonSerializer.Deserialize<ShoppingProductTestResponse>(body, JsonOptions);
        Assert.NotNull(product);
        return product!;
    }

    private async Task<ShoppingVariantTestResponse> CreateShoppingVariantAsync(HttpClient client, Guid productId, string name, decimal quantity, ShoppingUnit unit)
    {
        var response = await client.PostAsJsonAsync("/api/v1/shopping/variants", new { productId, name, normalizedQuantity = quantity, unit, barcode = (string?)null });
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, body);
        var variant = JsonSerializer.Deserialize<ShoppingVariantTestResponse>(body, JsonOptions);
        Assert.NotNull(variant);
        return variant!;
    }

    private async Task<(string Id, string PlainTextKey)> CreateApiKeyIdAsync(HttpClient client, string name)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/api-keys", new { name });
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, body);
        var created = await response.Content.ReadFromJsonAsync<ApiKeyCreatedResponse>(JsonOptions);
        Assert.NotNull(created);
        return (created!.Summary.Id, created.ApiKey);
    }

    private static async Task<byte[]> CreateDetailedJpegAsync(int width, int height)
    {
        using var image = new Image<Rgba32>(width, height);
        var random = new Random(12345);

        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (var x = 0; x < row.Length; x++)
                {
                    var blockNoise = (byte)random.Next(0, 80);
                    var red = (byte)((x * 255 / width + blockNoise) % 256);
                    var green = (byte)((y * 255 / height + blockNoise) % 256);
                    var blue = (byte)(((x + y) * 255 / (width + height) + blockNoise) % 256);
                    row[x] = new Rgba32(
                        red,
                        green,
                        blue,
                        byte.MaxValue);
                }
            }
        });

        await using var stream = new MemoryStream();
        await image.SaveAsJpegAsync(stream, new JpegEncoder { Quality = 92 });
        return stream.ToArray();
    }

    private sealed record AuthTestResponse(string AccessToken, DateTimeOffset ExpiresAt, AuthTestUser User);
    private sealed record AuthTestUser(Guid Id, string Email, string DisplayName);
    private sealed record ApiKeySummaryResponse(string Id, string Name, string Preview, DateTimeOffset CreatedAt, DateTimeOffset? LastUsedAt, DateTimeOffset? RevokedAt, bool IsRevoked);
    private sealed record ApiKeyCreatedResponse(string ApiKey, ApiKeySummaryResponse Summary);
    private sealed record FinancialGoalTestResponse(Guid Id, string Name, string? Description, decimal TargetAmount, decimal CurrentAmount, FinancialGoalType Type, FinancialGoalStatus Status, int Priority, string? Url);
    private sealed record FinancialGoalContributionTestResponse(Guid Id, Guid GoalId, Guid? TransactionId, Guid? AccountId, decimal Amount, DateTimeOffset ContributionDate);
    private sealed record BudgetTestResponse(Guid Id, string Name, decimal LimitAmount, PeriodType PeriodType, BudgetValidityType ValidityType, DateTimeOffset? PeriodStart, DateTimeOffset? PeriodEnd, bool IsActive, decimal UsedAmount, decimal RemainingAmount, decimal UsagePercent, int TransactionCount, bool IsOverLimit, DateTimeOffset CurrentPeriodStart, DateTimeOffset CurrentPeriodEnd);
    private sealed record BudgetHistoryTestResponse(DateTimeOffset PeriodStart, DateTimeOffset PeriodEnd, string GroupKey, decimal SpentAmount, decimal LimitAmount, decimal RemainingAmount, decimal UsagePercent, int TransactionCount, bool IsOverLimit);
    private sealed record ShoppingStoreTestResponse(Guid Id, string Name, string? Notes);
    private sealed record ShoppingProductTestResponse(Guid Id, string Name, Guid? CategoryId, string? Notes);
    private sealed record ShoppingVariantTestResponse(Guid Id, Guid ProductId, string ProductName, string Name, decimal NormalizedQuantity, ShoppingUnit Unit, string? Barcode);
    private sealed record ShoppingListTestResponse(Guid Id, string Name, DateTimeOffset ListDate);
    private sealed record ShoppingRecommendationTestResponse(Guid ShoppingListId, decimal EstimatedTotal, decimal EstimatedSavings, IReadOnlyList<ShoppingRecommendationGroupTestResponse> StoreGroups);
    private sealed record ShoppingRecommendationGroupTestResponse(Guid StoreId, string StoreName, decimal Subtotal);
}
