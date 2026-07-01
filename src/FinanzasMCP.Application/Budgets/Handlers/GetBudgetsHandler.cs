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
        var now = DateTimeOffset.UtcNow;
        var budgets = await dbContext.Set<Budget>().AsNoTracking().ToListAsync(cancellationToken);
        var budgetIds = budgets.Select(x => x.Id).ToArray();
        var transactions = await dbContext.Set<FinanzasMCP.Domain.Transactions.Transaction>()
            .AsNoTracking()
            .Where(x => x.BudgetId.HasValue && budgetIds.Contains(x.BudgetId.Value))
            .ToListAsync(cancellationToken);

        return budgets
            .OrderBy(x => x.PeriodStart)
            .Select(x => BudgetUsageCalculator.ToSummaryForPeriod(x, transactions, ResolvePeriod(x, query, now)))
            .ToArray();
    }

    private static (DateTimeOffset Start, DateTimeOffset End) ResolvePeriod(Budget budget, GetBudgetsQuery query, DateTimeOffset now)
    {
        if (query.DateFrom is null && query.DateTo is null)
        {
            return BudgetUsageCalculator.CurrentPeriod(budget, now);
        }

        var start = query.DateFrom ?? budget.PeriodStart ?? DateTimeOffset.MinValue;
        var end = query.DateTo ?? budget.PeriodEnd ?? now;

        if (budget.PeriodStart is not null && start < budget.PeriodStart.Value)
        {
            start = budget.PeriodStart.Value;
        }

        if (budget.PeriodEnd is not null && end > budget.PeriodEnd.Value)
        {
            end = budget.PeriodEnd.Value;
        }

        if (end < start)
        {
            end = start;
        }

        return (start, end);
    }
}
