using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class DeleteDebtHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteDebtCommand command, CancellationToken cancellationToken = default)
    {
        var debt = await dbContext.Set<Debt>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        debt.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
