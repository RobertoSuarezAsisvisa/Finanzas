using System.Security.Claims;
using System.Text.Encodings.Web;
using FinanzasMCP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinanzasMCP.McpServer.Auth;

public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    FinanzasMCPDbContext dbContext,
    ApiKeyTokenService apiKeyTokenService) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var rawKey = GetPresentedApiKey(Request);
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            return AuthenticateResult.NoResult();
        }

        if (!apiKeyTokenService.TryParse(rawKey, out var parsed))
        {
            return AuthenticateResult.Fail("Invalid API key format.");
        }

        var apiKey = await dbContext.UserApiKeys
            .IgnoreQueryFilters()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.LookupKey == parsed.LookupKey, Context.RequestAborted);

        if (apiKey is null || apiKey.IsDeleted || apiKey.IsRevoked || !apiKey.User.IsActive)
        {
            return AuthenticateResult.Fail("Invalid API key.");
        }

        if (!apiKeyTokenService.Verify(parsed.Secret, apiKey.SecretHash))
        {
            return AuthenticateResult.Fail("Invalid API key.");
        }

        apiKey.RegisterUse();
        await dbContext.SaveChangesAsync(Context.RequestAborted);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, apiKey.User.Id.ToString()),
            new Claim(ClaimTypes.Email, apiKey.User.Email),
            new Claim(ClaimTypes.Name, apiKey.User.DisplayName),
            new Claim("auth_method", "api_key"),
            new Claim("api_key_id", apiKey.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationDefaults.Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationDefaults.Scheme);
        return AuthenticateResult.Success(ticket);
    }

    private static string? GetPresentedApiKey(HttpRequest request)
    {
        if (request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var headerValue))
        {
            var value = headerValue.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        var authorization = request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(authorization) &&
            authorization.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase))
        {
            return authorization["ApiKey ".Length..].Trim();
        }

        return null;
    }
}
