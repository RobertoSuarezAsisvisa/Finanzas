using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Transactions.Commands;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Transactions.Handlers;

public sealed class CreateTransactionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<TransactionSummary> Handle(CreateTransactionCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive.");
        }

        var account = await dbContext.Accounts.FirstAsync(x => x.Id == command.AccountId, cancellationToken);
        Account? toAccount = null;
        if (command.ToAccountId is not null)
        {
            toAccount = await dbContext.Accounts.FirstAsync(x => x.Id == command.ToAccountId, cancellationToken);
        }

        var transaction = Transaction.Create(
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

        switch (command.Type)
        {
            case TransactionType.Income:
                account.Deposit(command.Amount);
                break;
            case TransactionType.Expense:
                account.Withdraw(command.Amount);
                break;
            case TransactionType.Transfer:
                account.Withdraw(command.Amount);
                toAccount?.Deposit(command.Amount);
                break;
        }

        dbContext.Set<Transaction>().Add(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TransactionSummary(
            transaction.Id,
            transaction.Type,
            transaction.Amount,
            transaction.Currency,
            transaction.AccountId,
            transaction.ToAccountId,
            transaction.CategoryId,
            transaction.Description,
            transaction.Reference,
            transaction.TransactionDate);
    }
}
