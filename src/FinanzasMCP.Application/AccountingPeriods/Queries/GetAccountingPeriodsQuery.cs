using FinanzasMCP.Domain.AccountingPeriods;

namespace FinanzasMCP.Application.AccountingPeriods.Queries;

public sealed record GetAccountingPeriodsQuery(AccountingPeriodStatus? Status = null);
