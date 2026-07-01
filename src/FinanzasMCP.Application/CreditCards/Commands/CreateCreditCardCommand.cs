using FinanzasMCP.Domain.CreditCards;

namespace FinanzasMCP.Application.CreditCards.Commands;

public sealed record CreateCreditCardCommand(
    string Name,
    string Currency,
    string Issuer,
    CreditCardBrand Brand,
    decimal CreditLimit,
    int StatementClosingDay,
    int PaymentDueDay,
    string? ProductName,
    string? LastFour,
    decimal OutstandingBalance,
    CreditCardPaymentMode PaymentMode,
    string? RewardsProgram,
    CreditCardStatementDelivery StatementDelivery,
    decimal? InterestNominalAnnual,
    decimal? InterestEffectiveAnnual);
