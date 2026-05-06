using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.PurchaseGoals.Commands;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Goals;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.PurchaseGoals.Handlers;

public sealed class AddPurchaseGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<PurchaseGoalSummary> Handle(AddPurchaseGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive.");
        }

        var goal = await dbContext.Set<PurchaseGoal>().FirstAsync(x => x.Id == command.PurchaseGoalId, cancellationToken);
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
                $"Aporte a meta de compra: {goal.Name}",
                $"purchase-goal:{goal.Id}",
                command.ContributionDate.ToUtcSafe(),
                null);

            sourceAccount.Withdraw(command.Amount);
            targetAccount?.Deposit(command.Amount);
            dbContext.Set<Transaction>().Add(transaction);
            transactionId = transaction.Id;
        }

        goal.AddContribution(command.Amount);
        dbContext.Set<PurchaseGoalContribution>().Add(PurchaseGoalContribution.Create(goal.Id, command.Amount, command.ContributionDate.ToUtcSafe(), transactionId));
        await dbContext.SaveChangesAsync(cancellationToken);

        return new PurchaseGoalSummary(goal.Id, goal.Name, goal.GoalPrice, goal.SavedAmount, goal.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow), goal.Priority, goal.Url, goal.AccountId, goal.TargetDate, goal.Status);
    }
}
