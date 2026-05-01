using FinanzasMCP.Domain.Crypto;

namespace FinanzasMCP.Application.Crypto.Commands;

public sealed record UpdateCryptoLotCommand(
    Guid Id,
    Guid AccountId,
    Guid? TransactionId,
    decimal Quantity,
    decimal BuyPriceUsd,
    decimal? SellPriceUsd,
    CryptoLotStatus Status,
    DateTimeOffset OperationDate);
