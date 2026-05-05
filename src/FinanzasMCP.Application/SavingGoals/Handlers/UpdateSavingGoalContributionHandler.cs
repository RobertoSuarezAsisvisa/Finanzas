using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class UpdateSavingGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(UpdateSavingGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive.");
        }

        var contribution = await dbContext.Set<SavingGoalContribution>()
            .Include(x => x.SavingGoal)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);
        var transactionId = command.TransactionId ?? contribution.TransactionId;

        if (transactionId is not null)
        {
            var transaction = await dbContext.Set<Transaction>()
                .Include(x => x.Account)
                .Include(x => x.ToAccount)
                .FirstAsync(x => x.Id == transactionId, cancellationToken);
            Reverse(transaction);

            var sourceAccountId = command.AccountId ?? transaction.AccountId;
            var sourceAccount = await dbContext.Accounts.FirstAsync(x => x.Id == sourceAccountId, cancellationToken);
            Account? targetAccount = null;
            if (contribution.SavingGoal.AccountId is not null && contribution.SavingGoal.AccountId != sourceAccountId)
            {
                targetAccount = await dbContext.Accounts.FirstAsync(x => x.Id == contribution.SavingGoal.AccountId, cancellationToken);
            }

            transaction.UpdateDetails(
                targetAccount is null ? TransactionType.Expense : TransactionType.Transfer,
                command.Amount,
                sourceAccount.Currency,
                sourceAccount.Id,
                targetAccount?.Id,
                null,
                $"Aporte a meta de ahorro: {contribution.SavingGoal.Name}",
                $"saving-goal:{contribution.SavingGoal.Id}",
                command.ContributionDate.ToUtcSafe(),
                null);
            Apply(transaction, sourceAccount, targetAccount);
        }

        contribution.SavingGoal.AdjustContribution(command.Amount - contribution.Amount);
        contribution.UpdateDetails(command.Amount, command.ContributionDate.ToUtcSafe(), transactionId);
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
