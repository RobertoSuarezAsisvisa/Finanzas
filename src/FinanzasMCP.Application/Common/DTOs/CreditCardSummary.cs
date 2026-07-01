using FinanzasMCP.Domain.CreditCards;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record CreditCardSummary(
    Guid Id,
    Guid AccountId,
    string AccountName,
    string Currency,
    string Issuer,
    CreditCardBrand Brand,
    string? ProductName,
    string? LastFour,
    decimal CreditLimit,
    decimal OutstandingBalance,
    decimal AvailableCredit,
    int StatementClosingDay,
    int PaymentDueDay,
    CreditCardPaymentMode PaymentMode,
    string? RewardsProgram,
    CreditCardStatementDelivery StatementDelivery,
    decimal? InterestNominalAnnual,
    decimal? InterestEffectiveAnnual,
    bool IsActive,
    DateTimeOffset? NextDueDate);
