namespace FinanzasMCP.Application.CryptoAccounts.Commands;

public sealed record CreateCryptoAccountCommand(
    Guid AccountId,
    string Symbol,
    string? Network,
    decimal Quantity,
    decimal? AvgBuyPriceUsd);
