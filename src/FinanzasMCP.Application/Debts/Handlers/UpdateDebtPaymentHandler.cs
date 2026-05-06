using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Debts;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class UpdateDebtPaymentHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(UpdateDebtPaymentCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive.");
        }

        var payment = await dbContext.Set<DebtPayment>()
            .Include(x => x.Debt)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);
        var transactionId = command.TransactionId ?? payment.TransactionId;

        if (transactionId is not null)
        {
            var transaction = await dbContext.Set<Transaction>()
                .Include(x => x.Account)
                .FirstAsync(x => x.Id == transactionId, cancellationToken);
            Reverse(transaction);

            var accountId = command.AccountId ?? transaction.AccountId;
            var account = await dbContext.Accounts.FirstAsync(x => x.Id == accountId, cancellationToken);
            var transactionType = payment.Debt.Type == DebtType.Payable ? TransactionType.Expense : TransactionType.Income;
            transaction.UpdateDetails(
                transactionType,
                command.Amount,
                account.Currency,
                account.Id,
                null,
                null,
                null,
                payment.Debt.Type == DebtType.Payable
                    ? $"Pago de deuda: {payment.Debt.ContactName}"
                    : $"Cobro de deuda: {payment.Debt.ContactName}",
                $"debt:{payment.Debt.Id}",
                command.PaymentDate.ToUtcSafe(),
                null);
            Apply(transactionType, account, command.Amount);
        }
        else if (command.AccountId is not null)
        {
            var account = await dbContext.Accounts.FirstAsync(x => x.Id == command.AccountId, cancellationToken);
            var transactionType = payment.Debt.Type == DebtType.Payable ? TransactionType.Expense : TransactionType.Income;
            var transaction = Transaction.Create(
                transactionType,
                command.Amount,
                account.Currency,
                account.Id,
                null,
                null,
                null,
                payment.Debt.Type == DebtType.Payable
                    ? $"Pago de deuda: {payment.Debt.ContactName}"
                    : $"Cobro de deuda: {payment.Debt.ContactName}",
                $"debt:{payment.Debt.Id}",
                command.PaymentDate.ToUtcSafe(),
                null);

            Apply(transactionType, account, command.Amount);
            dbContext.Set<Transaction>().Add(transaction);
            transactionId = transaction.Id;
        }

        payment.Debt.AdjustPayment(command.Amount - payment.Amount);
        payment.UpdateDetails(command.Amount, command.PaymentDate.ToUtcSafe(), command.Notes, transactionId);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Reverse(Transaction transaction)
    {
        switch (transaction.Type)
        {
            case TransactionType.Income:
                transaction.Account.Withdraw(transaction.Amount);
                break;
            case TransactionType.Expense:
                transaction.Account.Deposit(transaction.Amount);
                break;
            case TransactionType.Transfer:
                transaction.Account.Deposit(transaction.Amount);
                break;
        }
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
