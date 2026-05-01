using FinanzasMCP.Application.AccountingPeriods.Commands;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.AccountingPeriods;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.AccountingPeriods.Handlers;

public sealed class UpdateAccountingPeriodHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<AccountingPeriodSummary> Handle(UpdateAccountingPeriodCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new InvalidOperationException("Name is required.");
        }

        if (command.EndDate <= command.StartDate)
        {
            throw new InvalidOperationException("End date must be after start date.");
        }

        var period = await dbContext.Set<AccountingPeriod>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        period.UpdateDetails(
            command.Name,
            command.StartDate.ToUtcSafe(),
            command.EndDate.ToUtcSafe(),
            command.Status,
            command.TotalIncome,
            command.TotalExpenses,
            command.NetBalance,
            command.ClosedAt?.ToUtcSafe());

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AccountingPeriodSummary(
            period.Id,
            period.Name,
            period.StartDate,
            period.EndDate,
            period.TotalIncome,
            period.TotalExpenses,
            period.NetBalance,
            period.Status,
            period.ClosedAt);
    }
}
