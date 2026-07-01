using FinanzasMCP.Domain.CreditCards;

namespace FinanzasMCP.Application.CreditCards.Commands;

public sealed record UpdateCreditCardCommand(
    Guid Id,
    string Name,
    string Currency,
    string Issuer,
    CreditCardBrand Brand,
    decimal CreditLimit,
    int StatementClosingDay,
    int PaymentDueDay,
    string? ProductName,
    string? LastFour,
    CreditCardPaymentMode PaymentMode,
    string? RewardsProgram,
    CreditCardStatementDelivery StatementDelivery,
    decimal? InterestNominalAnnual,
    decimal? InterestEffectiveAnnual,
    bool IsActive);
