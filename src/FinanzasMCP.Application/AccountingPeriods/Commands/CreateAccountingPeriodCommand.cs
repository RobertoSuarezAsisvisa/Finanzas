namespace FinanzasMCP.Application.AccountingPeriods.Commands;

public sealed record CreateAccountingPeriodCommand(string Name, DateTimeOffset StartDate, DateTimeOffset EndDate);
