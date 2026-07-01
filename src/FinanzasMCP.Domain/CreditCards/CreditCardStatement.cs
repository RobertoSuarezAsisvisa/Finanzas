using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.CreditCards;

public sealed class CreditCardStatement : UserOwnedEntity
{
    public Guid CreditCardAccountId { get; private set; }
    public DateTimeOffset PeriodStart { get; private set; }
    public DateTimeOffset PeriodEnd { get; private set; }
    public DateTimeOffset StatementDate { get; private set; }
    public DateTimeOffset DueDate { get; private set; }
    public decimal OpeningBalance { get; private set; }
    public decimal Purchases { get; private set; }
    public decimal Fees { get; private set; }
    public decimal Interest { get; private set; }
    public decimal Payments { get; private set; }
    public decimal StatementBalance { get; private set; }
    public decimal MinimumPayment { get; private set; }
    public CreditCardStatementStatus Status { get; private set; }
    public CreditCardAccount CreditCardAccount { get; private set; } = null!;

    public static CreditCardStatement Create(
        Guid creditCardAccountId,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd,
        DateTimeOffset statementDate,
        DateTimeOffset dueDate,
        decimal openingBalance,
        decimal purchases,
        decimal fees,
        decimal interest,
        decimal payments,
        decimal minimumPayment)
    {
        var statementBalance = Math.Max(0m, openingBalance + purchases + fees + interest - payments);
        return new CreditCardStatement
        {
            CreditCardAccountId = creditCardAccountId,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            StatementDate = statementDate,
            DueDate = dueDate,
            OpeningBalance = openingBalance,
            Purchases = purchases,
            Fees = fees,
            Interest = interest,
            Payments = payments,
            StatementBalance = statementBalance,
            MinimumPayment = Math.Min(statementBalance, Math.Max(0m, minimumPayment)),
            Status = CreditCardStatementStatus.Closed
        };
    }

    public void MarkPaid()
    {
        Status = CreditCardStatementStatus.Paid;
        MarkUpdated();
    }
}
