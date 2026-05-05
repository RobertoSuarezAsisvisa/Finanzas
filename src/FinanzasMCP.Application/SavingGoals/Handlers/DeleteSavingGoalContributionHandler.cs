using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class DeleteSavingGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteSavingGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        var contribution = await dbContext.Set<SavingGoalContribution>()
            .Include(x => x.SavingGoal)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);
        if (contribution.TransactionId is not null)
        {
            var transaction = await dbContext.Set<Transaction>()
                .Include(x => x.Account)
                .Include(x => x.ToAccount)
                .FirstAsync(x => x.Id == contribution.TransactionId, cancellationToken);
            Reverse(transaction);
            transaction.SoftDelete();
        }

        contribution.SavingGoal.AdjustContribution(-contribution.Amount);
        contribution.SoftDelete();
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
