namespace FinanzasMCP.Application.Common.DTOs;

public sealed record BudgetUsageHistoryPoint(
    DateTimeOffset PeriodStart,
    DateTimeOffset PeriodEnd,
    string GroupKey,
    decimal SpentAmount,
    decimal LimitAmount,
    decimal RemainingAmount,
    decimal UsagePercent,
    int TransactionCount,
    bool IsOverLimit);
