using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.SavingGoals.Queries;
using FinanzasMCP.Domain.Contributions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class GetSavingGoalContributionsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<SavingGoalContributionSummary>> Handle(GetSavingGoalContributionsQuery query, CancellationToken cancellationToken = default)
    {
        var contributions = dbContext.Set<SavingGoalContribution>().AsNoTracking().AsQueryable();
        if (query.GoalId is not null)
        {
            contributions = contributions.Where(x => x.GoalId == query.GoalId);
        }

        var list = await contributions.OrderByDescending(x => x.ContributionDate).ToListAsync(cancellationToken);
        return list.Select(x => new SavingGoalContributionSummary(x.Id, x.GoalId, x.TransactionId, x.Amount, x.ContributionDate)).ToArray();
    }
}
