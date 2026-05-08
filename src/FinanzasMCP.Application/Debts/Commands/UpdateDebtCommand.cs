using FinanzasMCP.Domain.Debts;

namespace FinanzasMCP.Application.Debts.Commands;

public sealed record UpdateDebtCommand(Guid Id, DebtType Type, string ContactName, decimal OriginalAmount, decimal RemainingAmount, string Currency, DateTimeOffset? DueDate, Guid? AccountId, DebtStatus? Status, string? Notes, decimal? InterestRate, InterestPeriod? InterestPeriod, AmortizationMethod? AmortizationMethod, int? TermMonths, DateTimeOffset? LoanStartDate);
