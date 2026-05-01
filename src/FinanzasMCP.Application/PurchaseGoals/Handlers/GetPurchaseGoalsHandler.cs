using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.PurchaseGoals.Queries;
using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.PurchaseGoals.Handlers;

public sealed class GetPurchaseGoalsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<PurchaseGoalSummary>> Handle(GetPurchaseGoalsQuery query, CancellationToken cancellationToken = default)
    {
        var goals = await dbContext.Set<PurchaseGoal>()
            .AsNoTracking()
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return goals
            .Select(x => new PurchaseGoalSummary(x.Id, x.Name, x.GoalPrice, x.SavedAmount, x.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow), x.Priority, x.Url, x.AccountId, x.TargetDate, x.Status))
            .ToArray();
    }
}
