using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.CreditCards.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.CreditCards;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.CreditCards.Handlers;

public sealed class CloseCreditCardStatementHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<CreditCardStatementSummary> Handle(CloseCreditCardStatementCommand command, CancellationToken cancellationToken = default)
    {
        if (command.PeriodEnd < command.PeriodStart)
        {
            throw new InvalidOperationException("Period end must be after period start.");
        }

        var creditCard = await dbContext.CreditCardAccounts.FirstAsync(x => x.Id == command.CreditCardId, cancellationToken);
        var movements = await dbContext.CreditCardTransactions
            .Include(x => x.Transaction)
            .Where(x => x.CreditCardAccountId == command.CreditCardId)
            .Where(x => x.Transaction.TransactionDate >= command.PeriodStart && x.Transaction.TransactionDate <= command.PeriodEnd)
            .ToListAsync(cancellationToken);

        var purchases = movements
            .Where(x => x.OperationType is CreditCardOperationType.Purchase or CreditCardOperationType.CashAdvance)
            .Sum(x => x.Transaction.Amount);
        var fees = movements.Where(x => x.OperationType == CreditCardOperationType.Fee).Sum(x => x.Transaction.Amount);
        var interest = movements.Where(x => x.OperationType == CreditCardOperationType.Interest).Sum(x => x.Transaction.Amount);
        var payments = movements
            .Where(x => x.OperationType is CreditCardOperationType.Payment or CreditCardOperationType.Refund)
            .Sum(x => x.Transaction.Amount);

        var statementDate = command.StatementDate ?? command.PeriodEnd;
        var dueDate = command.DueDate ?? BuildDueDate(statementDate, creditCard.PaymentDueDay);
        var openingBalance = Math.Max(0m, creditCard.OutstandingBalance - purchases - fees - interest + payments);
        var statementBalance = Math.Max(0m, openingBalance + purchases + fees + interest - payments);
        var minimumPayment = command.MinimumPayment ?? Math.Round(statementBalance * 0.10m, 2, MidpointRounding.AwayFromZero);

        var statement = CreditCardStatement.Create(
            creditCard.Id,
            command.PeriodStart,
            command.PeriodEnd,
            statementDate,
            dueDate,
            openingBalance,
            purchases,
            fees,
            interest,
            payments,
            minimumPayment);

        dbContext.CreditCardStatements.Add(statement);
        foreach (var movement in movements.Where(x => x.StatementId is null))
        {
            movement.UpdateDetails(
                movement.CreditCardAccountId,
                movement.OperationType,
                statement.Id,
                movement.IsForeign,
                movement.InstallmentCount,
                movement.Merchant);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return CreditCardMapping.ToStatementSummary(statement);
    }

    private static DateTimeOffset BuildDueDate(DateTimeOffset statementDate, int paymentDueDay)
    {
        var dueMonth = statementDate.AddMonths(1);
        var day = Math.Min(paymentDueDay, DateTime.DaysInMonth(dueMonth.Year, dueMonth.Month));
        return new DateTimeOffset(dueMonth.Year, dueMonth.Month, day, 0, 0, 0, statementDate.Offset);
    }
}
