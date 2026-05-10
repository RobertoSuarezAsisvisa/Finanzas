using FinanzasMCP.Application;
using FinanzasMCP.Application.Auth;
using FinanzasMCP.Infrastructure;
using FinanzasMCP.Infrastructure.Persistence;
using FinanzasMCP.McpServer.Api;
using FinanzasMCP.McpServer.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FinanzasMCP.McpServer.Tools;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<JwtTokenService>();
builder.Services.AddSingleton<FirebaseAuthService>();
builder.Services.AddScoped<LegacyDataClaimer>();

var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];
if (!string.IsNullOrWhiteSpace(jwtSigningKey))
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
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
    .WithTools<SavingGoalTools>()
    .WithTools<PurchaseGoalTools>()
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

app.MapMcp("/mcp");
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

    var credential = TryGetGoogleCredential(configuration);
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

static GoogleCredential? TryGetGoogleCredential(IConfiguration configuration)
{
    try
    {
        var serviceAccountPath = configuration["Firebase:ServiceAccountPath"];
        if (!string.IsNullOrWhiteSpace(serviceAccountPath))
        {
            return GoogleCredential.FromFile(Environment.ExpandEnvironmentVariables(serviceAccountPath));
        }

        return GoogleCredential.GetApplicationDefault();
    }
    catch
    {
        return null;
    }
}
