using FinanzasMCP.Application.Budgets.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Budgets;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Budgets.Handlers;

public sealed class DeleteBudgetHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteBudgetCommand command, CancellationToken cancellationToken = default)
    {
        var budget = await dbContext.Set<Budget>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        budget.Deactivate();
        budget.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
