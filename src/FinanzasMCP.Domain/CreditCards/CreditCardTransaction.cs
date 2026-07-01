using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Domain.CreditCards;

public sealed class CreditCardTransaction : UserOwnedEntity
{
    public Guid TransactionId { get; private set; }
    public Guid CreditCardAccountId { get; private set; }
    public CreditCardOperationType OperationType { get; private set; }
    public Guid? StatementId { get; private set; }
    public bool IsForeign { get; private set; }
    public int? InstallmentCount { get; private set; }
    public string? Merchant { get; private set; }
    public Transaction Transaction { get; private set; } = null!;
    public CreditCardAccount CreditCardAccount { get; private set; } = null!;
    public CreditCardStatement? Statement { get; private set; }

    public static CreditCardTransaction Create(
        Guid transactionId,
        Guid creditCardAccountId,
        CreditCardOperationType operationType,
        Guid? statementId = null,
        bool isForeign = false,
        int? installmentCount = null,
        string? merchant = null)
    {
        if (installmentCount is <= 0) throw new InvalidOperationException("Installment count must be positive.");

        return new CreditCardTransaction
        {
            TransactionId = transactionId,
            CreditCardAccountId = creditCardAccountId,
            OperationType = operationType,
            StatementId = statementId,
            IsForeign = isForeign,
            InstallmentCount = installmentCount,
            Merchant = merchant?.Trim()
        };
    }

    public void UpdateDetails(
        Guid creditCardAccountId,
        CreditCardOperationType operationType,
        Guid? statementId,
        bool isForeign,
        int? installmentCount,
        string? merchant)
    {
        if (installmentCount is <= 0) throw new InvalidOperationException("Installment count must be positive.");

        CreditCardAccountId = creditCardAccountId;
        OperationType = operationType;
        StatementId = statementId;
        IsForeign = isForeign;
        InstallmentCount = installmentCount;
        Merchant = merchant?.Trim();
        MarkUpdated();
    }
}
