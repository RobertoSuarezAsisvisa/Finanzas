using FinanzasMCP.Application.AccountingPeriods.Commands;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.AccountingPeriods;

namespace FinanzasMCP.Application.AccountingPeriods.Handlers;

public sealed class CreateAccountingPeriodHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<AccountingPeriodSummary> Handle(CreateAccountingPeriodCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new InvalidOperationException("Name is required.");
        }

        if (command.EndDate <= command.StartDate)
        {
            throw new InvalidOperationException("End date must be after start date.");
        }

        var period = AccountingPeriod.Create(command.Name, command.StartDate.ToUtcSafe(), command.EndDate.ToUtcSafe());
        dbContext.Set<AccountingPeriod>().Add(period);
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
