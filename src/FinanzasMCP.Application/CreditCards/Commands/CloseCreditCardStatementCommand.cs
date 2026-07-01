namespace FinanzasMCP.Application.CreditCards.Commands;

public sealed record CloseCreditCardStatementCommand(
    Guid CreditCardId,
    DateTimeOffset PeriodStart,
    DateTimeOffset PeriodEnd,
    DateTimeOffset? StatementDate,
    DateTimeOffset? DueDate,
    decimal? MinimumPayment);
