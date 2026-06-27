using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Goals.Handlers;

public sealed class DeleteFinancialGoalHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteFinancialGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = await dbContext.Set<FinancialGoal>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        goal.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
