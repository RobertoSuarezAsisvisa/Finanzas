using FinanzasMCP.Domain.Accounts;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record AccountSummary(
    Guid Id,
    string Name,
    AccountType AccountType,
    string Currency,
    AccountPurpose Purpose,
    decimal Balance,
    bool IsActive,
    string? BankName,
    string? AccountNumber,
    string? Provider,
    string? CryptoSymbol,
    string? CryptoNetwork,
    decimal? CryptoQuantity,
    decimal? CryptoAvgBuyPriceUsd);
