namespace FinanzasMCP.Application.Common.DTOs;

public sealed record UserContextEntrySummary(Guid Id, string Key, string Value, DateTimeOffset UpdatedAt);
