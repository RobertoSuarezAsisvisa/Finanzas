using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.PurchaseGoals.Commands;
using FinanzasMCP.Domain.Contributions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.PurchaseGoals.Handlers;

public sealed class UpdatePurchaseGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(UpdatePurchaseGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        var contribution = await dbContext.Set<PurchaseGoalContribution>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        contribution.UpdateDetails(command.Amount, command.ContributionDate.ToUtcSafe(), command.TransactionId);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
