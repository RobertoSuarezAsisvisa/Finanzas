using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Transactions.Commands;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Transactions.Handlers;

public sealed class UpdateTransactionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<TransactionSummary> Handle(UpdateTransactionCommand command, CancellationToken cancellationToken = default)
    {
        var transaction = await dbContext.Set<Transaction>()
            .Include(x => x.Account)
            .Include(x => x.ToAccount)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);

        Revert(transaction);

        var account = await dbContext.Accounts.FirstAsync(x => x.Id == command.AccountId, cancellationToken);
        Account? toAccount = null;
        if (command.ToAccountId is not null)
        {
            toAccount = await dbContext.Accounts.FirstAsync(x => x.Id == command.ToAccountId, cancellationToken);
        }

        transaction.UpdateDetails(
            command.Type,
            command.Amount,
            command.Currency,
            command.AccountId,
            command.ToAccountId,
            command.CategoryId,
            command.Description,
            command.Reference,
            command.TransactionDate.ToUtcSafe(),
            command.RecurringRuleId);

        Apply(transaction, account, toAccount);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TransactionSummary(transaction.Id, transaction.Type, transaction.Amount, transaction.Currency, transaction.AccountId, transaction.ToAccountId, transaction.CategoryId, transaction.Description, transaction.Reference, transaction.TransactionDate);
    }

    private static void Revert(Transaction transaction)
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
                transaction.ToAccount?.Withdraw(transaction.Amount);
                break;
        }
    }

    private static void Apply(Transaction transaction, Account account, Account? toAccount)
    {
        switch (transaction.Type)
        {
            case TransactionType.Income:
                account.Deposit(transaction.Amount);
                break;
            case TransactionType.Expense:
                account.Withdraw(transaction.Amount);
                break;
            case TransactionType.Transfer:
                account.Withdraw(transaction.Amount);
                toAccount?.Deposit(transaction.Amount);
                break;
        }
    }
}
