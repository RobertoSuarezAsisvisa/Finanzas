using FinanzasMCP.Domain.AccountingPeriods;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record AccountingPeriodSummary(
    Guid Id,
    string Name,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal NetBalance,
    AccountingPeriodStatus Status,
    DateTimeOffset? ClosedAt);
