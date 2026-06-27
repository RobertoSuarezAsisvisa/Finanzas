using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Goals;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Goals.Handlers;

public sealed class AddFinancialGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<FinancialGoalSummary> Handle(AddFinancialGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive.");
        }

        var goal = await dbContext.Set<FinancialGoal>().FirstAsync(x => x.Id == command.GoalId, cancellationToken);
        var transactionId = command.TransactionId;

        if (transactionId is null)
        {
            if (command.AccountId is null)
            {
                throw new InvalidOperationException("Debit account is required when no transaction is provided.");
            }

            var sourceAccount = await dbContext.Accounts.FirstAsync(x => x.Id == command.AccountId, cancellationToken);
            Account? targetAccount = null;
            if (goal.AccountId is not null && goal.AccountId != command.AccountId)
            {
                targetAccount = await dbContext.Accounts.FirstAsync(x => x.Id == goal.AccountId, cancellationToken);
            }

            var transaction = Transaction.Create(
                targetAccount is null ? TransactionType.Expense : TransactionType.Transfer,
                command.Amount,
                sourceAccount.Currency,
                sourceAccount.Id,
                targetAccount?.Id,
                null,
                null,
                $"Aporte a objetivo financiero: {goal.Name}",
                $"financial-goal:{goal.Id}",
                command.ContributionDate.ToUtcSafe(),
                null);

            sourceAccount.Withdraw(command.Amount);
            targetAccount?.Deposit(command.Amount);
            dbContext.Set<Transaction>().Add(transaction);
            transactionId = transaction.Id;
        }

        goal.AddContribution(command.Amount);
        dbContext.Set<FinancialGoalContribution>().Add(FinancialGoalContribution.Create(goal.Id, command.Amount, command.ContributionDate.ToUtcSafe(), transactionId));
        await dbContext.SaveChangesAsync(cancellationToken);

        return FinancialGoalMapping.ToSummary(goal);
    }
}
