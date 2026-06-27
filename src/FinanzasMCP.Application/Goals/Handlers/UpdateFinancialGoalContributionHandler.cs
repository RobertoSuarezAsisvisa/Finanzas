using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Goals.Handlers;

public sealed class UpdateFinancialGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(UpdateFinancialGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive.");
        }

        var contribution = await dbContext.Set<FinancialGoalContribution>()
            .Include(x => x.Goal)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);
        var transactionId = command.TransactionId ?? contribution.TransactionId;

        if (transactionId is not null)
        {
            var transaction = await dbContext.Set<Transaction>()
                .Include(x => x.Account)
                .Include(x => x.ToAccount)
                .FirstAsync(x => x.Id == transactionId, cancellationToken);
            TransactionBalanceAdjustment.Reverse(transaction);

            var sourceAccountId = command.AccountId ?? transaction.AccountId;
            var sourceAccount = await dbContext.Accounts.FirstAsync(x => x.Id == sourceAccountId, cancellationToken);
            Account? targetAccount = null;
            if (contribution.Goal.AccountId is not null && contribution.Goal.AccountId != sourceAccountId)
            {
                targetAccount = await dbContext.Accounts.FirstAsync(x => x.Id == contribution.Goal.AccountId, cancellationToken);
            }

            transaction.UpdateDetails(
                targetAccount is null ? TransactionType.Expense : TransactionType.Transfer,
                command.Amount,
                sourceAccount.Currency,
                sourceAccount.Id,
                targetAccount?.Id,
                null,
                null,
                $"Aporte a objetivo financiero: {contribution.Goal.Name}",
                $"financial-goal:{contribution.Goal.Id}",
                command.ContributionDate.ToUtcSafe(),
                null);
            TransactionBalanceAdjustment.Apply(transaction, sourceAccount, targetAccount);
        }
        else if (command.AccountId is not null)
        {
            var sourceAccount = await dbContext.Accounts.FirstAsync(x => x.Id == command.AccountId, cancellationToken);
            Account? targetAccount = null;
            if (contribution.Goal.AccountId is not null && contribution.Goal.AccountId != command.AccountId)
            {
                targetAccount = await dbContext.Accounts.FirstAsync(x => x.Id == contribution.Goal.AccountId, cancellationToken);
            }

            var transaction = Transaction.Create(
                targetAccount is null ? TransactionType.Expense : TransactionType.Transfer,
                command.Amount,
                sourceAccount.Currency,
                sourceAccount.Id,
                targetAccount?.Id,
                null,
                null,
                $"Aporte a objetivo financiero: {contribution.Goal.Name}",
                $"financial-goal:{contribution.Goal.Id}",
                command.ContributionDate.ToUtcSafe(),
                null);

            TransactionBalanceAdjustment.Apply(transaction, sourceAccount, targetAccount);
            dbContext.Set<Transaction>().Add(transaction);
            transactionId = transaction.Id;
        }

        contribution.Goal.AdjustContribution(command.Amount - contribution.Amount);
        contribution.UpdateDetails(command.Amount, command.ContributionDate.ToUtcSafe(), transactionId);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
