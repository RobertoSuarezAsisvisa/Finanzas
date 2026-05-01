using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.PurchaseGoals.Commands;
using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.PurchaseGoals.Handlers;

public sealed class CreatePurchaseGoalHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<PurchaseGoalSummary> Handle(CreatePurchaseGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = PurchaseGoal.Create(command.Name, command.TargetPrice, command.Description, command.Priority, command.Url, command.AccountId, command.TargetDate);
        dbContext.Set<PurchaseGoal>().Add(goal);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new PurchaseGoalSummary(goal.Id, goal.Name, goal.GoalPrice, goal.SavedAmount, goal.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow), goal.Priority, goal.Url, goal.AccountId, goal.TargetDate, goal.Status);
    }
}
