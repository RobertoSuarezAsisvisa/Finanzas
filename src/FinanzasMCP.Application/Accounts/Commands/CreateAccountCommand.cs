using FinanzasMCP.Domain.Accounts;

namespace FinanzasMCP.Application.Accounts.Commands;

public sealed record CreateAccountCommand(
    string Name,
    AccountType AccountType,
    string Currency,
    decimal Balance,
    string? BankName,
    string? AccountNumber,
    string? Provider,
    string? CryptoSymbol,
    string? CryptoNetwork,
    decimal? CryptoQuantity,
    decimal? CryptoAvgBuyPriceUsd);
