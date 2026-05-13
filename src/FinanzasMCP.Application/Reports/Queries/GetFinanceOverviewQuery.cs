namespace FinanzasMCP.Application.Reports.Queries;

public sealed record GetFinanceOverviewQuery(DateTimeOffset? DateFrom = null, DateTimeOffset? DateTo = null);
