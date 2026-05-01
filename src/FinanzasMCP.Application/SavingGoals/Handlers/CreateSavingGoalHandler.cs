using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class CreateSavingGoalHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<SavingGoalSummary> Handle(CreateSavingGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = SavingGoal.Create(command.Name, command.TargetAmount, command.AccountId, command.TargetDate);
        dbContext.Set<SavingGoal>().Add(goal);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SavingGoalSummary(goal.Id, goal.Name, goal.GoalAmount, goal.CurrentAmount, goal.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow), goal.AccountId, goal.GoalDate, goal.Status);
    }
}
