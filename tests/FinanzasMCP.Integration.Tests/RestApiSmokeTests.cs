using System.Net;
using System.Net.Http.Json;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Domain.Accounts;
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
    public async Task Cors_allows_gcs_frontend_origin()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/health");
        request.Headers.Add("Origin", "https://storage.googleapis.com");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins));
        Assert.Contains("https://storage.googleapis.com", origins);
    }

    [Fact]
    public async Task Accounts_and_user_context_flow_works()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();

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

        var response = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Expense",
            amount = -1m,
            currency = "USD",
            accountId = account!.Id,
            toAccountId = (Guid?)null,
            categoryId = (Guid?)null,
            description = "Invalid",
            reference = (string?)null,
            transactionDate = DateTimeOffset.UtcNow,
            recurringRuleId = (Guid?)null,
            tagIds = Array.Empty<Guid>()
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Account_can_be_deleted()
    {
        factory.InitializeDatabase();
        var client = factory.CreateClient();

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
}
