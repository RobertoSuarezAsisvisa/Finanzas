using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.PurchaseGoals.Commands;
using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.PurchaseGoals.Handlers;

public sealed class DeletePurchaseGoalHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeletePurchaseGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = await dbContext.Set<PurchaseGoal>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        goal.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
