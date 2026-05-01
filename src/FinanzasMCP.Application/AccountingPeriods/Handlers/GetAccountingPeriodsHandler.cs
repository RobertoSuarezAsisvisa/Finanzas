using FinanzasMCP.Application.AccountingPeriods.Queries;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.AccountingPeriods;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.AccountingPeriods.Handlers;

public sealed class GetAccountingPeriodsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<AccountingPeriodSummary>> Handle(GetAccountingPeriodsQuery query, CancellationToken cancellationToken = default)
    {
        var periods = dbContext.Set<AccountingPeriod>().AsQueryable();
        if (query.Status is not null)
        {
            periods = periods.Where(x => x.Status == query.Status);
        }

        return await periods
            .OrderByDescending(x => x.StartDate)
            .Select(period => new AccountingPeriodSummary(
                period.Id,
                period.Name,
                period.StartDate,
                period.EndDate,
                period.TotalIncome,
                period.TotalExpenses,
                period.NetBalance,
                period.Status,
                period.ClosedAt))
            .ToListAsync(cancellationToken);
    }
}
