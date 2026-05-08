using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Debts;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class RegisterDebtPaymentHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<DebtSummary> Handle(RegisterDebtPaymentCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive.");
        }

        var debt = await dbContext.Set<Debt>().FirstAsync(x => x.Id == command.DebtId, cancellationToken);
        var transactionId = command.TransactionId;

        if (transactionId is null)
        {
            if (command.AccountId is null)
            {
                throw new InvalidOperationException("Account is required when no transaction is provided.");
            }

            var account = await dbContext.Accounts.FirstAsync(x => x.Id == command.AccountId, cancellationToken);
            var transactionType = debt.Type == DebtType.Payable ? TransactionType.Expense : TransactionType.Income;
            var transaction = Transaction.Create(
                transactionType,
                command.Amount,
                account.Currency,
                account.Id,
                null,
                null,
                null,
                debt.Type == DebtType.Payable
                    ? $"Pago de deuda: {debt.ContactName}"
                    : $"Cobro de deuda: {debt.ContactName}",
                $"debt:{debt.Id}",
                command.PaymentDate.ToUtcSafe(),
                null);

            Apply(transactionType, account, command.Amount);
            dbContext.Set<Transaction>().Add(transaction);
            transactionId = transaction.Id;
        }

        debt.RegisterPayment(command.Amount);
        dbContext.Set<DebtPayment>().Add(DebtPayment.Create(debt.Id, command.Amount, command.PaymentDate.ToUtcSafe(), command.Notes, transactionId));
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DebtSummary(debt.Id, debt.Type, debt.ContactName, debt.OriginalAmount, debt.RemainingAmount, debt.Currency, debt.DueDate, debt.AccountId, debt.Status, debt.Notes, debt.InterestRate, debt.InterestPeriod, debt.AmortizationMethod, debt.TermMonths, debt.LoanStartDate);
    }

    private static void Apply(TransactionType transactionType, Account account, decimal amount)
    {
        if (transactionType == TransactionType.Income)
        {
            account.Deposit(amount);
            return;
        }

        account.Withdraw(amount);
    }
}
