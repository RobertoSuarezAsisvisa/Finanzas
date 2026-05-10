using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Users;

public sealed class UserExternalLogin : Entity
{
    public Guid UserId { get; private set; }
    public string Provider { get; private set; } = string.Empty;
    public string ProviderUserId { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastLoginAt { get; private set; } = DateTimeOffset.UtcNow;
    public AppUser User { get; private set; } = null!;

    public static UserExternalLogin Create(Guid userId, string provider, string providerUserId, string? email)
        => new()
        {
            UserId = userId,
            Provider = provider.Trim().ToLowerInvariant(),
            ProviderUserId = providerUserId.Trim(),
            Email = email?.Trim().ToLowerInvariant()
        };

    public void RegisterLogin()
    {
        LastLoginAt = DateTimeOffset.UtcNow;
    }
}
