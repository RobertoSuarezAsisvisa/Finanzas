using FinanzasMCP.Domain.Debts;

namespace FinanzasMCP.Application.Debts.Commands;

public sealed record CreateDebtCommand(DebtType Type, string ContactName, decimal OriginalAmount, decimal RemainingAmount, string Currency, DateTimeOffset? DueDate, Guid? AccountId, string? Notes);
