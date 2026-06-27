using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Goals.Queries;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Contributions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Goals.Handlers;

public sealed class GetFinancialGoalContributionsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<FinancialGoalContributionSummary>> Handle(GetFinancialGoalContributionsQuery query, CancellationToken cancellationToken = default)
    {
        var contributions = dbContext.Set<FinancialGoalContribution>()
            .AsNoTracking()
            .Include(x => x.Transaction)
            .AsQueryable();

        if (query.GoalId is not null)
        {
            contributions = contributions.Where(x => x.GoalId == query.GoalId);
        }

        var ordered = await contributions.ToListAsync(cancellationToken);

        return ordered
            .OrderByDescending(x => x.ContributionDate)
            .ThenBy(x => x.Id)
            .Select(FinancialGoalMapping.ToSummary)
            .ToArray();
    }
}
