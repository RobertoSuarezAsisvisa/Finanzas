namespace FinanzasMCP.Application.Budgets.Queries;

public sealed record GetBudgetUsageHistoryQuery(Guid BudgetId, BudgetUsageGroupBy GroupBy, DateTimeOffset? DateFrom = null, DateTimeOffset? DateTo = null);
