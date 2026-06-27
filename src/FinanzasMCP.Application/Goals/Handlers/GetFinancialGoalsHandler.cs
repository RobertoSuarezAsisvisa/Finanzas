using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Goals.Queries;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Goals.Handlers;

public sealed class GetFinancialGoalsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<FinancialGoalSummary>> Handle(GetFinancialGoalsQuery query, CancellationToken cancellationToken = default)
    {
        var goals = dbContext.Set<FinancialGoal>().AsNoTracking().AsQueryable();
        if (query.Type is not null)
        {
            goals = goals.Where(x => x.Type == query.Type);
        }

        var ordered = await goals
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return ordered.Select(FinancialGoalMapping.ToSummary).ToArray();
    }
}
