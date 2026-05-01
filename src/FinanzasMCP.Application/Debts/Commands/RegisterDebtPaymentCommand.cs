namespace FinanzasMCP.Application.Debts.Commands;

public sealed record RegisterDebtPaymentCommand(Guid DebtId, decimal Amount, DateTimeOffset PaymentDate, string? Notes, Guid? TransactionId);
