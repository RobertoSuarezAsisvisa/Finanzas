using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.PurchaseGoals.Commands;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Goals;
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
        goal.AddContribution(command.Amount);
        dbContext.Set<PurchaseGoalContribution>().Add(PurchaseGoalContribution.Create(goal.Id, command.Amount, command.ContributionDate.ToUtcSafe(), command.TransactionId));
        await dbContext.SaveChangesAsync(cancellationToken);

        return new PurchaseGoalSummary(goal.Id, goal.Name, goal.GoalPrice, goal.SavedAmount, goal.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow), goal.Priority, goal.Url, goal.AccountId, goal.TargetDate, goal.Status);
    }
}
