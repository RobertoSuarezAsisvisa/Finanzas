using FinanzasMCP.Domain.AccountingPeriods;

namespace FinanzasMCP.Application.AccountingPeriods.Commands;

public sealed record UpdateAccountingPeriodCommand(
    Guid Id,
    string Name,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    AccountingPeriodStatus Status,
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal NetBalance,
    DateTimeOffset? ClosedAt);
