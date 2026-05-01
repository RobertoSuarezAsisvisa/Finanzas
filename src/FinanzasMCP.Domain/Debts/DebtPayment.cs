using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Debts;

public sealed class DebtPayment : SoftDeletableEntity
{
    public Guid DebtId { get; private set; }
    public Guid? TransactionId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset PaymentDate { get; private set; }
    public string? Notes { get; private set; }

    public Debt Debt { get; private set; } = null!;

    public static DebtPayment Create(Guid debtId, decimal amount, DateTimeOffset paymentDate, string? notes = null, Guid? transactionId = null)
        => new()
        {
            DebtId = debtId,
            Amount = amount,
            PaymentDate = paymentDate,
            Notes = notes?.Trim(),
            TransactionId = transactionId
        };

    public void UpdateDetails(decimal amount, DateTimeOffset paymentDate, string? notes = null, Guid? transactionId = null)
    {
        Amount = amount;
        PaymentDate = paymentDate;
        Notes = notes?.Trim();
        TransactionId = transactionId;
        MarkUpdated();
    }
}
