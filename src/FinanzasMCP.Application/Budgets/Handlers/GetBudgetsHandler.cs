using FinanzasMCP.Application.Budgets.Queries;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Budgets;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Budgets.Handlers;

public sealed class GetBudgetsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<BudgetSummary>> Handle(GetBudgetsQuery query, CancellationToken cancellationToken = default)
    {
        var budgets = await dbContext.Set<Budget>().AsNoTracking().OrderBy(x => x.PeriodStart).ToListAsync(cancellationToken);
        return budgets.Select(x => new BudgetSummary(x.Id, x.Name, x.CategoryId, x.LimitAmount, x.PeriodType, x.ValidityType, x.PeriodStart, x.PeriodEnd, x.IsActive)).ToArray();
    }
}
