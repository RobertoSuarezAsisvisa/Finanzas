using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Domain.Accounts;
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

        var createTransactionResponse = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Expense",
            amount = 25m,
            currency = "USD",
            accountId = account!.Id,
            toAccountId = (Guid?)null,
            categoryId = (Guid?)null,
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

        var createTransactionResponse = await client.PostAsJsonAsync("/api/v1/transactions", new
        {
            type = "Expense",
            amount = 42m,
            currency = "USD",
            accountId = account!.Id,
            toAccountId = (Guid?)null,
            categoryId = (Guid?)null,
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

    private static async Task AuthenticateAsync(HttpClient client)
    {
        var email = $"test-{Guid.NewGuid():N}@finanzas.local";
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email,
            password = "Password123!",
            displayName = "Test User"
        });

        response.EnsureSuccessStatusCode();
        var auth = await response.Content.ReadFromJsonAsync<AuthTestResponse>(JsonOptions);
        Assert.NotNull(auth);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);
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
}
