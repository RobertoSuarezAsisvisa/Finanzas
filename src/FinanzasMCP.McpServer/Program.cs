using FinanzasMCP.Application;
using FinanzasMCP.Application.Auth;
using FinanzasMCP.Infrastructure;
using FinanzasMCP.Infrastructure.Persistence;
using FinanzasMCP.McpServer.Api;
using FinanzasMCP.McpServer.Auth;
using FinanzasMCP.McpServer.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FinanzasMCP.McpServer.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
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
builder.Services.AddScoped<LegacyDataClaimer>();
builder.Services.AddScoped<OAuthUserMapper>();

var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];
var oauthAuthority = NormalizeAuthority(builder.Configuration["OAuth:Authority"]);
var oauthAudience = builder.Configuration["OAuth:Audience"];
var hasOAuthConfiguration = !string.IsNullOrWhiteSpace(oauthAuthority) &&
    !string.IsNullOrWhiteSpace(oauthAudience);

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

            if (hasOAuthConfiguration &&
                TryGetBearerToken(authorization, out var bearerToken) &&
                TokenIssuerMatches(bearerToken, oauthAuthority))
            {
                return AuthSchemeNames.OAuthBearer;
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

if (hasOAuthConfiguration)
{
    builder.Services.AddAuthentication()
        .AddJwtBearer(AuthSchemeNames.OAuthBearer, options =>
        {
            options.Authority = oauthAuthority;
            options.Audience = oauthAudience;
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = oauthAuthority,
                ValidateAudience = true,
                ValidAudience = oauthAudience,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                NameClaimType = "name",
                RoleClaimType = "permissions",
                ClockSkew = TimeSpan.FromMinutes(1)
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    try
                    {
                        var mapper = context.HttpContext.RequestServices.GetRequiredService<OAuthUserMapper>();
                        var principal = await mapper.MapAsync(context.Principal!, context.HttpContext.RequestAborted);
                        context.Principal = principal;
                    }
                    catch (InvalidOperationException ex)
                    {
                        context.Fail(ex);
                    }
                }
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
app.UseMiddleware<McpOAuthScopeMiddleware>();

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

static bool TryGetBearerToken(string? authorization, out string token)
{
    token = string.Empty;
    if (string.IsNullOrWhiteSpace(authorization) ||
        !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }

    token = authorization["Bearer ".Length..].Trim();
    return token.Length > 0;
}

static bool TokenIssuerMatches(string token, string? expectedIssuer)
{
    if (string.IsNullOrWhiteSpace(expectedIssuer))
    {
        return false;
    }

    try
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return string.Equals(NormalizeAuthority(jwt.Issuer), expectedIssuer, StringComparison.OrdinalIgnoreCase);
    }
    catch (ArgumentException)
    {
        return false;
    }
}

static string? NormalizeAuthority(string? authority)
{
    if (string.IsNullOrWhiteSpace(authority))
    {
        return null;
    }

    return authority.Trim().TrimEnd('/') + "/";
}
