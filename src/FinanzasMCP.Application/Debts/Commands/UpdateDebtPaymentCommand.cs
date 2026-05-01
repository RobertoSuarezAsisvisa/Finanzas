namespace FinanzasMCP.Application.Debts.Commands;

public sealed record UpdateDebtPaymentCommand(Guid Id, decimal Amount, DateTimeOffset PaymentDate, string? Notes, Guid? TransactionId);
