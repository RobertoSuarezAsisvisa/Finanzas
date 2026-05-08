namespace FinanzasMCP.Application.Common.DTOs;

public sealed record DebtInstallmentSummary(
    Guid Id,
    Guid DebtId,
    int Number,
    DateTimeOffset DueDate,
    decimal ExpectedPayment,
    decimal Principal,
    decimal Interest,
    decimal PaidAmount,
    decimal PendingAmount,
    decimal BalanceAfterPayment,
    string Status,
    int DaysOverdue);
