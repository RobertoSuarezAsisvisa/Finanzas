using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record FinancialGoalSummary(
    Guid Id,
    string Name,
    string? Description,
    decimal TargetAmount,
    decimal CurrentAmount,
    decimal? SuggestedMonthlyContribution,
    Guid? AccountId,
    DateTimeOffset? TargetDate,
    FinancialGoalStatus Status,
    FinancialGoalType Type,
    int Priority,
    string? Url,
    DateTimeOffset? CompletedAt);
