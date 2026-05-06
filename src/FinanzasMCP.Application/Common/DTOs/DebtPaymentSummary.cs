namespace FinanzasMCP.Application.Common.DTOs;

public sealed record DebtPaymentSummary(Guid Id, Guid DebtId, Guid? TransactionId, Guid? AccountId, decimal Amount, DateTimeOffset PaymentDate, string? Notes);
