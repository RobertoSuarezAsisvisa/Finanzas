using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Transactions.Commands;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Transactions.Handlers;

public sealed class DeleteTransactionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteTransactionCommand command, CancellationToken cancellationToken = default)
    {
        var transaction = await dbContext.Set<Transaction>()
            .Include(x => x.Account)
            .Include(x => x.ToAccount)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);

        Reverse(transaction);
        transaction.SoftDelete();
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
                transaction.ToAccount?.Withdraw(transaction.Amount);
                break;
        }
    }
}
