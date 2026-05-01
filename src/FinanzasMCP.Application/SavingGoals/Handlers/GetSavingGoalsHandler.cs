using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.SavingGoals.Queries;
using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class GetSavingGoalsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<SavingGoalSummary>> Handle(GetSavingGoalsQuery query, CancellationToken cancellationToken = default)
    {
        var goals = await dbContext.Set<SavingGoal>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return goals
            .Select(x => new SavingGoalSummary(x.Id, x.Name, x.GoalAmount, x.CurrentAmount, x.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow), x.AccountId, x.GoalDate, x.Status))
            .ToArray();
    }
}
