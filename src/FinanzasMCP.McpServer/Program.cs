using FinanzasMCP.Application;
using FinanzasMCP.Infrastructure;
using FinanzasMCP.McpServer.Tools;
using ModelContextProtocol.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options => options.Stateless = true)
    .WithTools<AccountTools>()
    .WithTools<CryptoAccountTools>()
    .WithTools<CategoryTools>()
    .WithTools<TagTools>()
    .WithTools<TransactionTools>()
    .WithTools<BudgetTools>()
    .WithTools<CryptoLotTools>()
    .WithTools<AccountingPeriodTools>()
    .WithTools<SavingGoalTools>()
    .WithTools<PurchaseGoalTools>()
    .WithTools<DebtTools>()
    .WithTools<DebtPaymentTools>()
    .WithTools<RecurringRuleTools>()
    .WithTools<UserContextTools>()
    .WithTools<ReportTools>();

var app = builder.Build();
app.MapMcp("/mcp");

static object CreateAuthorizationServerMetadata(HttpRequest request)
{
    var baseUri = $"{request.Scheme}://{request.Host}";

    return new
    {
        issuer = baseUri,
        authorization_endpoint = $"{baseUri}/authorize",
        token_endpoint = $"{baseUri}/token",
        registration_endpoint = $"{baseUri}/register",
        response_types_supported = new[] { "code" },
        grant_types_supported = new[] { "authorization_code", "refresh_token" },
        code_challenge_methods_supported = new[] { "S256" }
    };
}

app.MapGet("/.well-known/oauth-authorization-server/mcp", (HttpRequest request) =>
{
    return Results.Json(CreateAuthorizationServerMetadata(request));
});

app.MapGet("/.well-known/oauth-authorization-server", (HttpRequest request) =>
{
    return Results.Json(CreateAuthorizationServerMetadata(request));
});

app.Run();
