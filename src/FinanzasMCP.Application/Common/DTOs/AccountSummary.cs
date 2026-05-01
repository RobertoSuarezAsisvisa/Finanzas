using FinanzasMCP.Domain.Accounts;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record AccountSummary(
    Guid Id,
    string Name,
    AccountType AccountType,
    string Currency,
    decimal Balance,
    bool IsActive,
    string? BankName,
    string? Provider,
    string? CryptoSymbol,
    string? CryptoNetwork,
    decimal? CryptoQuantity,
    decimal? CryptoAvgBuyPriceUsd);
