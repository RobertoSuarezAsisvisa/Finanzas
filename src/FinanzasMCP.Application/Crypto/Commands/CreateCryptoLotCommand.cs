using FinanzasMCP.Domain.Crypto;

namespace FinanzasMCP.Application.Crypto.Commands;

public sealed record CreateCryptoLotCommand(
    Guid AccountId,
    Guid? TransactionId,
    decimal Quantity,
    decimal BuyPriceUsd,
    decimal? SellPriceUsd,
    CryptoLotStatus Status,
    DateTimeOffset OperationDate);
