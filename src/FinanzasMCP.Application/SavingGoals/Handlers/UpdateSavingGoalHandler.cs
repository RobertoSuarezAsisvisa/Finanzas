using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class UpdateSavingGoalHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<SavingGoalSummary> Handle(UpdateSavingGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = await dbContext.Set<SavingGoal>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        goal.UpdateDetails(command.Name, command.TargetAmount, command.AccountId, command.TargetDate, command.Status);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SavingGoalSummary(goal.Id, goal.Name, goal.GoalAmount, goal.CurrentAmount, goal.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow), goal.AccountId, goal.GoalDate, goal.Status);
    }
}
