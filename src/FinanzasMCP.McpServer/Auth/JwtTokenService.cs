using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinanzasMCP.Domain.Users;
using Microsoft.IdentityModel.Tokens;

namespace FinanzasMCP.McpServer.Auth;

public sealed class JwtTokenService(IConfiguration configuration)
{
    public AuthToken CreateToken(AppUser user)
    {
        var key = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Missing Jwt:SigningKey configuration.");

        if (key.Length < 32)
        {
            throw new InvalidOperationException("Jwt:SigningKey must be at least 32 characters.");
        }

        var expiresAt = DateTimeOffset.UtcNow.AddHours(12);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.DisplayName)
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new AuthToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

public sealed record AuthToken(string AccessToken, DateTimeOffset ExpiresAt);
