using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.PurchaseGoals.Commands;
using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.PurchaseGoals.Handlers;

public sealed class UpdatePurchaseGoalHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<PurchaseGoalSummary> Handle(UpdatePurchaseGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = await dbContext.Set<PurchaseGoal>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        goal.UpdateDetails(command.Name, command.TargetPrice, command.Description, command.Priority, command.Url, command.AccountId, command.TargetDate, command.Status, command.PurchasedAt);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new PurchaseGoalSummary(goal.Id, goal.Name, goal.GoalPrice, goal.SavedAmount, goal.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow), goal.Priority, goal.Url, goal.AccountId, goal.TargetDate, goal.Status);
    }
}
