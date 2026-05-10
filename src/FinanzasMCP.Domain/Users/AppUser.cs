using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Users;

public sealed class AppUser : Entity
{
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? PasswordHash { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public ICollection<UserExternalLogin> ExternalLogins { get; private set; } = new List<UserExternalLogin>();

    public static AppUser Create(string email, string displayName, string? passwordHash = null)
        => new()
        {
            Email = NormalizeEmail(email),
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? NormalizeEmail(email) : displayName.Trim(),
            PasswordHash = passwordHash
        };

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        MarkUpdated();
    }

    public void UpdateProfile(string displayName)
    {
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? Email : displayName.Trim();
        MarkUpdated();
    }

    public void RegisterLogin()
    {
        LastLoginAt = DateTimeOffset.UtcNow;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }

    private void MarkUpdated() => UpdatedAt = DateTimeOffset.UtcNow;

    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
