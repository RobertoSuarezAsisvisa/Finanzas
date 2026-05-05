using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Goals;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class AddSavingGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<SavingGoalSummary> Handle(AddSavingGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive.");
        }

        var goal = await dbContext.Set<SavingGoal>().FirstAsync(x => x.Id == command.GoalId, cancellationToken);
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
                $"Aporte a meta de ahorro: {goal.Name}",
                $"saving-goal:{goal.Id}",
                command.ContributionDate.ToUtcSafe(),
                null);

            sourceAccount.Withdraw(command.Amount);
            targetAccount?.Deposit(command.Amount);
            dbContext.Set<Transaction>().Add(transaction);
            transactionId = transaction.Id;
        }

        goal.AddContribution(command.Amount);
        dbContext.Set<SavingGoalContribution>().Add(SavingGoalContribution.Create(goal.Id, command.Amount, command.ContributionDate.ToUtcSafe(), transactionId));
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SavingGoalSummary(goal.Id, goal.Name, goal.GoalAmount, goal.CurrentAmount, goal.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow), goal.AccountId, goal.GoalDate, goal.Status);
    }
}
