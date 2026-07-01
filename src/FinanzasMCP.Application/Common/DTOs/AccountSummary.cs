using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.CreditCards;

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
    decimal? CryptoAvgBuyPriceUsd,
    Guid? CreditCardId,
    string? CreditCardIssuer,
    CreditCardBrand? CreditCardBrand,
    string? CreditCardProductName,
    string? CreditCardLastFour,
    decimal? CreditLimit,
    decimal? OutstandingBalance,
    decimal? AvailableCredit,
    int? StatementClosingDay,
    int? PaymentDueDay,
    CreditCardPaymentMode? PaymentMode,
    string? RewardsProgram,
    CreditCardStatementDelivery? StatementDelivery,
    decimal? InterestNominalAnnual,
    decimal? InterestEffectiveAnnual);
