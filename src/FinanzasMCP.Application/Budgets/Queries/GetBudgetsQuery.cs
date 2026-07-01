namespace FinanzasMCP.Application.Budgets.Queries;

public sealed record GetBudgetsQuery(DateTimeOffset? DateFrom = null, DateTimeOffset? DateTo = null);
