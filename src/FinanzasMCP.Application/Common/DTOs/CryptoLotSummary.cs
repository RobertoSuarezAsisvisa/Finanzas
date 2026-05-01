using FinanzasMCP.Domain.Crypto;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record CryptoLotSummary(
    Guid Id,
    Guid AccountId,
    Guid? TransactionId,
    decimal Quantity,
    decimal BuyPriceUsd,
    decimal? SellPriceUsd,
    CryptoLotStatus Status,
    DateTimeOffset OperationDate);
