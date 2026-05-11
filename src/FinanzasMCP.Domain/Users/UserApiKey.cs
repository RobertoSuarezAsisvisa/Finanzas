using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Users;

public sealed class UserApiKey : SoftDeletableEntity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string LookupKey { get; private set; } = string.Empty;
    public string SecretHash { get; private set; } = string.Empty;
    public DateTimeOffset? LastUsedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public AppUser User { get; private set; } = null!;

    public bool IsRevoked => RevokedAt is not null;

    public static UserApiKey Create(Guid userId, string name, string lookupKey, string secretHash)
        => new()
        {
            UserId = userId,
            Name = NormalizeName(name),
            LookupKey = lookupKey.Trim().ToLowerInvariant(),
            SecretHash = secretHash.Trim()
        };

    public void Rename(string name)
    {
        Name = NormalizeName(name);
        MarkUpdated();
    }

    public void Revoke()
    {
        if (IsRevoked)
        {
            return;
        }

        RevokedAt = DateTimeOffset.UtcNow;
        MarkUpdated();
    }

    public void RegisterUse()
    {
        LastUsedAt = DateTimeOffset.UtcNow;
        MarkUpdated();
    }

    private static string NormalizeName(string name)
    {
        var normalized = name.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new InvalidOperationException("API key name is required.");
        }

        return normalized;
    }
}
