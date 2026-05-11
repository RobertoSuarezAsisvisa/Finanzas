using System.Security.Cryptography;
using FinanzasMCP.Application.Auth;

namespace FinanzasMCP.McpServer.Auth;

public sealed class ApiKeyTokenService(PasswordHasher passwordHasher) : IApiKeyTokenService
{
    public GeneratedApiKey Create()
    {
        var lookupKey = Convert.ToHexString(RandomNumberGenerator.GetBytes(8)).ToLowerInvariant();
        var secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(24)).ToLowerInvariant();
        var plainTextKey = $"fmcp_{lookupKey}_{secret}";
        return new GeneratedApiKey(plainTextKey, lookupKey, passwordHasher.Hash(secret));
    }

    public bool TryParse(string value, out ParsedApiKey parsed)
    {
        parsed = default!;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Trim().Split('_', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3 || !string.Equals(parts[0], "fmcp", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var lookupKey = parts[1].Trim().ToLowerInvariant();
        var secret = parts[2].Trim();
        if (lookupKey.Length < 8 || secret.Length < 16)
        {
            return false;
        }

        parsed = new ParsedApiKey(lookupKey, secret);
        return true;
    }

    public bool Verify(string secret, string secretHash) => passwordHasher.Verify(secret, secretHash);
}
