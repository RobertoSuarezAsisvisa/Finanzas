using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinanzasMCP.Domain.Users;
using FinanzasMCP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.McpServer.Auth;

public sealed class OAuthUserMapper(FinanzasMCPDbContext dbContext)
{
    private const string Provider = "auth0";

    public async Task<ClaimsPrincipal> MapAsync(ClaimsPrincipal externalPrincipal, CancellationToken cancellationToken)
    {
        var providerUserId = externalPrincipal.FindFirstValue("sub")
            ?? throw new InvalidOperationException("OAuth token does not include a subject.");
        var email = externalPrincipal.FindFirstValue("email");
        var normalizedEmail = string.IsNullOrWhiteSpace(email)
            ? null
            : AppUser.NormalizeEmail(email);

        var externalLogin = await dbContext.UserExternalLogins
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Provider == Provider && x.ProviderUserId == providerUserId, cancellationToken);

        AppUser user;
        if (externalLogin is not null)
        {
            externalLogin.RegisterLogin();
            user = externalLogin.User;
        }
        else
        {
            user = normalizedEmail is null
                ? AppUser.Create(CreateSyntheticEmail(providerUserId), GetDisplayName(externalPrincipal, providerUserId))
                : await dbContext.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken)
                    ?? AppUser.Create(normalizedEmail, GetDisplayName(externalPrincipal, normalizedEmail));

            if (dbContext.Entry(user).State == EntityState.Detached)
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            dbContext.UserExternalLogins.Add(UserExternalLogin.Create(user.Id, Provider, providerUserId, normalizedEmail));
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("The mapped Finanzas user is inactive.");
        }

        user.RegisterLogin();
        await dbContext.SaveChangesAsync(cancellationToken);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.DisplayName),
            new("auth_method", "oauth"),
            new("oauth_provider", Provider),
            new("oauth_subject", providerUserId)
        };

        claims.AddRange(externalPrincipal.Claims.Where(claim =>
            claim.Type is not ClaimTypes.NameIdentifier and not ClaimTypes.Email and not ClaimTypes.Name));

        var identity = new ClaimsIdentity(claims, AuthSchemeNames.OAuthBearer);
        return new ClaimsPrincipal(identity);
    }

    private static string GetDisplayName(ClaimsPrincipal principal, string fallback)
    {
        var name = principal.FindFirstValue("name");
        return string.IsNullOrWhiteSpace(name) ? fallback : name.Trim();
    }

    private static string CreateSyntheticEmail(string providerUserId)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(providerUserId));
        return $"{Provider}-{Convert.ToHexString(hash).ToLowerInvariant()}@oauth.local";
    }
}
