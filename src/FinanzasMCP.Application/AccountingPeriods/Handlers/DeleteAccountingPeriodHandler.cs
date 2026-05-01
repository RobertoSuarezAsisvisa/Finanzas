using FinanzasMCP.Application.AccountingPeriods.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.AccountingPeriods;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.AccountingPeriods.Handlers;

public sealed class DeleteAccountingPeriodHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteAccountingPeriodCommand command, CancellationToken cancellationToken = default)
    {
        var period = await dbContext.Set<AccountingPeriod>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        period.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
