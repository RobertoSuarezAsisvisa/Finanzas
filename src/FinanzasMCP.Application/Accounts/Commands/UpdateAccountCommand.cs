using FinanzasMCP.Domain.Accounts;

namespace FinanzasMCP.Application.Accounts.Commands;

public sealed record UpdateAccountCommand(
    Guid Id,
    string Name,
    AccountType AccountType,
    string Currency,
    AccountPurpose Purpose,
    decimal Balance,
    string? BankName,
    string? AccountNumber,
    string? Provider,
    string? CryptoSymbol,
    string? CryptoNetwork,
    decimal? CryptoQuantity,
    decimal? CryptoAvgBuyPriceUsd,
    bool IsActive);
