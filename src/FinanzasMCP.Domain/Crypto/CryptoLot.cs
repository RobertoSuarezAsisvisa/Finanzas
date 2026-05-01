using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Crypto;

public sealed class CryptoLot : SoftDeletableEntity
{
    public Guid AccountId { get; private set; }
    public Guid? TransactionId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal BuyPriceUsd { get; private set; }
    public decimal? SellPriceUsd { get; private set; }
    public CryptoLotStatus Status { get; private set; }
    public DateTimeOffset OperationDate { get; private set; }

    public CryptoAccount Account { get; private set; } = null!;

    public static CryptoLot Create(Guid accountId, Guid? transactionId, decimal quantity, decimal buyPriceUsd, decimal? sellPriceUsd, CryptoLotStatus status, DateTimeOffset operationDate)
        => new()
        {
            AccountId = accountId,
            TransactionId = transactionId,
            Quantity = quantity,
            BuyPriceUsd = buyPriceUsd,
            SellPriceUsd = sellPriceUsd,
            Status = status,
            OperationDate = operationDate
        };

    public void UpdateDetails(Guid accountId, Guid? transactionId, decimal quantity, decimal buyPriceUsd, decimal? sellPriceUsd, CryptoLotStatus status, DateTimeOffset operationDate)
    {
        AccountId = accountId;
        TransactionId = transactionId;
        Quantity = quantity;
        BuyPriceUsd = buyPriceUsd;
        SellPriceUsd = sellPriceUsd;
        Status = status;
        OperationDate = operationDate;
        MarkUpdated();
    }
}
