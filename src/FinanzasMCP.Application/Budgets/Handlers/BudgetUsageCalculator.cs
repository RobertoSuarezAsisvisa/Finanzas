using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Domain.Budgets;
using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Application.Budgets.Handlers;

internal static class BudgetUsageCalculator
{
    public static BudgetSummary ToSummary(Budget budget, IReadOnlyCollection<Transaction> transactions, DateTimeOffset now)
    {
        var period = CurrentPeriod(budget, now);
        return ToSummaryForPeriod(budget, transactions, period);
    }

    public static BudgetSummary ToSummaryForPeriod(Budget budget, IReadOnlyCollection<Transaction> transactions, (DateTimeOffset Start, DateTimeOffset End) period)
    {
        var periodTransactions = transactions
            .Where(transaction => transaction.BudgetId == budget.Id)
            .Where(transaction => transaction.Type == TransactionType.Expense)
            .Where(transaction => transaction.TransactionDate >= period.Start && transaction.TransactionDate <= period.End)
            .ToArray();
        var used = periodTransactions.Sum(transaction => transaction.Amount);

        return ToSummary(budget, used, periodTransactions.Length, period);
    }

    public static BudgetUsageHistoryPoint ToHistoryPoint(Budget budget, DateTimeOffset periodStart, DateTimeOffset periodEnd, IReadOnlyCollection<Transaction> transactions)
    {
        var spent = transactions
            .Where(transaction => transaction.Type == TransactionType.Expense)
            .Where(transaction => transaction.BudgetId == budget.Id)
            .Where(transaction => transaction.TransactionDate >= periodStart && transaction.TransactionDate <= periodEnd)
            .Sum(transaction => transaction.Amount);
        var remaining = budget.LimitAmount - spent;

        return new BudgetUsageHistoryPoint(
            periodStart,
            periodEnd,
            GroupKey(periodStart, periodEnd),
            spent,
            budget.LimitAmount,
            remaining,
            budget.LimitAmount > 0 ? Math.Round((spent / budget.LimitAmount) * 100m, 2) : 0m,
            transactions.Count(transaction => transaction.Type == TransactionType.Expense && transaction.BudgetId == budget.Id && transaction.TransactionDate >= periodStart && transaction.TransactionDate <= periodEnd),
            spent > budget.LimitAmount);
    }

    public static (DateTimeOffset Start, DateTimeOffset End) CurrentPeriod(Budget budget, DateTimeOffset now)
    {
        var start = budget.PeriodType switch
        {
            PeriodType.Daily => new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero),
            PeriodType.Weekly => StartOfWeek(now),
            PeriodType.Monthly => new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero),
            PeriodType.Yearly => new DateTimeOffset(now.Year, 1, 1, 0, 0, 0, TimeSpan.Zero),
            _ => new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero)
        };

        var end = budget.PeriodType switch
        {
            PeriodType.Daily => start.AddDays(1).AddTicks(-1),
            PeriodType.Weekly => start.AddDays(7).AddTicks(-1),
            PeriodType.Monthly => start.AddMonths(1).AddTicks(-1),
            PeriodType.Yearly => start.AddYears(1).AddTicks(-1),
            _ => start.AddDays(1).AddTicks(-1)
        };

        if (budget.PeriodStart is not null && start < budget.PeriodStart.Value)
        {
            start = budget.PeriodStart.Value;
        }

        if (budget.PeriodEnd is not null && end > budget.PeriodEnd.Value)
        {
            end = budget.PeriodEnd.Value;
        }

        return (start, end);
    }

    public static IEnumerable<(DateTimeOffset Start, DateTimeOffset End)> EnumeratePeriods(DateTimeOffset start, DateTimeOffset end, TimeSpan step)
    {
        for (var current = start; current <= end; current = current.Add(step))
        {
            var periodEnd = current.Add(step).AddTicks(-1);
            yield return (current, periodEnd > end ? end : periodEnd);
        }
    }

    public static DateTimeOffset StartOfWeek(DateTimeOffset date)
    {
        var dayOffset = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var monday = date.AddDays(-dayOffset);
        return new DateTimeOffset(monday.Year, monday.Month, monday.Day, 0, 0, 0, TimeSpan.Zero);
    }

    public static BudgetSummary ToSummary(Budget budget, decimal usedAmount, int transactionCount, (DateTimeOffset Start, DateTimeOffset End) period)
    {
        var remaining = budget.LimitAmount - usedAmount;

        return new BudgetSummary(
            budget.Id,
            budget.Name,
            budget.LimitAmount,
            budget.PeriodType,
            budget.ValidityType,
            budget.PeriodStart,
            budget.PeriodEnd,
            budget.IsActive,
            usedAmount,
            remaining,
            budget.LimitAmount > 0 ? Math.Round((usedAmount / budget.LimitAmount) * 100m, 2) : 0m,
            transactionCount,
            usedAmount > budget.LimitAmount,
            period.Start,
            period.End);
    }

    private static string GroupKey(DateTimeOffset periodStart, DateTimeOffset periodEnd)
        => periodStart.Date == periodEnd.Date
            ? periodStart.ToString("yyyy-MM-dd")
            : $"{periodStart:yyyy-MM-dd}/{periodEnd:yyyy-MM-dd}";
}
