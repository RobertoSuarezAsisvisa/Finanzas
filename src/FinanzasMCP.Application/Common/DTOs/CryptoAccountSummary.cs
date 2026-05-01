namespace FinanzasMCP.Application.Common.DTOs;

public sealed record CryptoAccountSummary(
    Guid Id,
    Guid AccountId,
    string Symbol,
    string? Network,
    decimal Quantity,
    decimal? AvgBuyPriceUsd);
