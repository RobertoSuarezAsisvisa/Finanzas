using FinanzasMCP.Application;
using FinanzasMCP.Application.Auth;
using FinanzasMCP.Application.Shopping;
using FinanzasMCP.Infrastructure;
using FinanzasMCP.Infrastructure.Persistence;
using FinanzasMCP.McpServer.Api;
using FinanzasMCP.McpServer.Auth;
using FinanzasMCP.McpServer.Shopping;
using FinanzasMCP.McpServer.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FinanzasMCP.McpServer.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicyName = "AllowAnyOrigin";

builder.Configuration.AddUserSecrets<Program>(optional: true);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        if (allowedOrigins.Length == 0)
        {
            allowedOrigins = ["http://localhost:4200", "http://127.0.0.1:4200"];
        }

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.Configure<TransactionAttachmentImageOptions>(
    builder.Configuration.GetSection(TransactionAttachmentImageOptions.SectionName));
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<ApiKeyTokenService>();
builder.Services.AddSingleton<IApiKeyTokenService>(sp => sp.GetRequiredService<ApiKeyTokenService>());
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<FirebaseAuthService>();
builder.Services.AddSingleton<GoogleCredentialResolver>();
builder.Services.AddSingleton<ITransactionAttachmentProcessor, TransactionAttachmentProcessor>();
builder.Services.AddSingleton<ITransactionAttachmentStorage, TransactionAttachmentStorage>();
builder.Services.AddHttpClient<IReceiptParser, GeminiReceiptParser>();
builder.Services.AddScoped<LegacyDataClaimer>();

var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "DynamicAuth";
        options.DefaultAuthenticateScheme = "DynamicAuth";
        options.DefaultChallengeScheme = "DynamicAuth";
    })
    .AddPolicyScheme("DynamicAuth", "JWT or API key", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            if (context.Request.Headers.ContainsKey(ApiKeyAuthenticationDefaults.HeaderName))
            {
                return ApiKeyAuthenticationDefaults.Scheme;
            }

            var authorization = context.Request.Headers.Authorization.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(authorization) &&
                authorization.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase))
            {
                return ApiKeyAuthenticationDefaults.Scheme;
            }

            return AuthSchemeNames.AppJwtBearer;
        };
    })
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationDefaults.Scheme, _ => { });

if (!string.IsNullOrWhiteSpace(jwtSigningKey))
{
    builder.Services.AddAuthentication()
        .AddJwtBearer(AuthSchemeNames.AppJwtBearer, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = !string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Issuer"]),
                ValidateAudience = !string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Audience"]),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });
}

builder.Services.AddAuthorization();
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
    .WithTools<GoalTools>()
    .WithTools<SavingGoalTools>()
    .WithTools<PurchaseGoalTools>()
    .WithTools<ContributionTools>()
    .WithTools<DebtTools>()
    .WithTools<DebtPaymentTools>()
    .WithTools<RecurringRuleTools>()
    .WithTools<UserContextTools>()
    .WithTools<ReportTools>();

var app = builder.Build();

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseCors(CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

InitializeFirebase(builder.Configuration, app.Environment);

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FinanzasMCPDbContext>();
    dbContext.Database.Migrate();
}

app.MapGet("/.well-known/openai-apps-challenge", (IConfiguration configuration) =>
{
    var token = configuration["OpenAIApps:DomainVerificationToken"];
    return string.IsNullOrWhiteSpace(token)
        ? Results.NotFound(new { message = "OpenAI Apps domain verification token is not configured." })
        : Results.Text(token, "text/plain");
}).AllowAnonymous();

app.MapMcp("/mcp").RequireAuthorization();
app.MapFinanzasRestApi();

app.Run();

static void InitializeFirebase(IConfiguration configuration, IHostEnvironment environment)
{
    if (environment.IsEnvironment("Testing") ||
        FirebaseApp.DefaultInstance is not null ||
        string.IsNullOrWhiteSpace(configuration["Firebase:ProjectId"]))
    {
        return;
    }

    var credential = new GoogleCredentialResolver(configuration).Resolve();
    if (credential is null)
    {
        return;
    }

    FirebaseApp.Create(new AppOptions
    {
        ProjectId = configuration["Firebase:ProjectId"],
        Credential = credential
    });
}
