using FinanzasMCP.Domain.CreditCards;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record CreditCardStatementSummary(
    Guid Id,
    Guid CreditCardAccountId,
    DateTimeOffset PeriodStart,
    DateTimeOffset PeriodEnd,
    DateTimeOffset StatementDate,
    DateTimeOffset DueDate,
    decimal OpeningBalance,
    decimal Purchases,
    decimal Fees,
    decimal Interest,
    decimal Payments,
    decimal StatementBalance,
    decimal MinimumPayment,
    CreditCardStatementStatus Status);
