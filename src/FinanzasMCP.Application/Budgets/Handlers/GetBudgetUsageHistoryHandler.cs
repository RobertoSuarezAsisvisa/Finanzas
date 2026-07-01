using FinanzasMCP.Application.Budgets.Queries;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Budgets;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Budgets.Handlers;

public sealed class GetBudgetUsageHistoryHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<BudgetUsageHistoryPoint>> Handle(GetBudgetUsageHistoryQuery query, CancellationToken cancellationToken = default)
    {
        var budget = await dbContext.Set<Budget>().AsNoTracking().FirstAsync(x => x.Id == query.BudgetId, cancellationToken);
        var range = ResolveRange(budget, query);
        var transactions = (await dbContext.Set<Transaction>()
            .AsNoTracking()
            .Where(x => x.BudgetId == budget.Id)
            .ToListAsync(cancellationToken))
            .Where(x => x.TransactionDate >= range.Start && x.TransactionDate <= range.End)
            .ToArray();

        return EnumeratePeriods(range.Start, range.End, query.GroupBy)
            .Select(period => BudgetUsageCalculator.ToHistoryPoint(budget, period.Start, period.End, transactions))
            .ToArray();
    }

    private static (DateTimeOffset Start, DateTimeOffset End) ResolveRange(Budget budget, GetBudgetUsageHistoryQuery query)
    {
        var now = DateTimeOffset.UtcNow;
        var defaultStart = query.GroupBy switch
        {
            BudgetUsageGroupBy.Day => now.AddDays(-30),
            BudgetUsageGroupBy.Week => now.AddDays(-7 * 12),
            BudgetUsageGroupBy.Month => now.AddMonths(-12),
            _ => now.AddDays(-30)
        };

        var start = query.DateFrom ?? budget.PeriodStart ?? new DateTimeOffset(defaultStart.Year, defaultStart.Month, defaultStart.Day, 0, 0, 0, TimeSpan.Zero);
        var end = query.DateTo ?? budget.PeriodEnd ?? now;

        if (end < start)
        {
            throw new InvalidOperationException("History end date must be later than start date.");
        }

        return (AlignStart(start, query.GroupBy), end);
    }

    private static IEnumerable<(DateTimeOffset Start, DateTimeOffset End)> EnumeratePeriods(DateTimeOffset start, DateTimeOffset end, BudgetUsageGroupBy groupBy)
    {
        if (groupBy == BudgetUsageGroupBy.Month)
        {
            for (var current = new DateTimeOffset(start.Year, start.Month, 1, 0, 0, 0, TimeSpan.Zero); current <= end; current = current.AddMonths(1))
            {
                var periodEnd = current.AddMonths(1).AddTicks(-1);
                yield return (current, periodEnd > end ? end : periodEnd);
            }

            yield break;
        }

        var step = groupBy == BudgetUsageGroupBy.Week ? TimeSpan.FromDays(7) : TimeSpan.FromDays(1);
        foreach (var period in BudgetUsageCalculator.EnumeratePeriods(start, end, step))
        {
            yield return period;
        }
    }

    private static DateTimeOffset AlignStart(DateTimeOffset start, BudgetUsageGroupBy groupBy)
        => groupBy switch
        {
            BudgetUsageGroupBy.Week => BudgetUsageCalculator.StartOfWeek(start),
            BudgetUsageGroupBy.Month => new DateTimeOffset(start.Year, start.Month, 1, 0, 0, 0, TimeSpan.Zero),
            _ => new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, TimeSpan.Zero)
        };
}
