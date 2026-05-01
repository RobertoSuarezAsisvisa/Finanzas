namespace FinanzasMCP.Application.CryptoAccounts.Commands;

public sealed record UpdateCryptoAccountCommand(
    Guid Id,
    Guid AccountId,
    string Symbol,
    string? Network,
    decimal Quantity,
    decimal? AvgBuyPriceUsd);
