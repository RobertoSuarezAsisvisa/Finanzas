using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Goals;
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
        goal.AddContribution(command.Amount);
        dbContext.Set<SavingGoalContribution>().Add(SavingGoalContribution.Create(goal.Id, command.Amount, command.ContributionDate.ToUtcSafe(), command.TransactionId));
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SavingGoalSummary(goal.Id, goal.Name, goal.GoalAmount, goal.CurrentAmount, goal.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow), goal.AccountId, goal.GoalDate, goal.Status);
    }
}
