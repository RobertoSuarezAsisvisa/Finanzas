namespace FinanzasMCP.Application.Auth;

public sealed record UserApiKeySummary(
    Guid Id,
    string Name,
    string Preview,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastUsedAt,
    DateTimeOffset? RevokedAt,
    bool IsRevoked);
