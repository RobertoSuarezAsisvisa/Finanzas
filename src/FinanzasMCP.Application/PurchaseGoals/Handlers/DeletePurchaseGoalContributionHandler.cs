using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.PurchaseGoals.Commands;
using FinanzasMCP.Domain.Contributions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.PurchaseGoals.Handlers;

public sealed class DeletePurchaseGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeletePurchaseGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        var contribution = await dbContext.Set<PurchaseGoalContribution>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        contribution.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
