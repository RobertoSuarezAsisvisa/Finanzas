using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Domain.Contributions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class UpdateSavingGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(UpdateSavingGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        var contribution = await dbContext.Set<SavingGoalContribution>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        contribution.UpdateDetails(command.Amount, command.ContributionDate.ToUtcSafe(), command.TransactionId);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
