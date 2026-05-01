using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Domain.Contributions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class DeleteSavingGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteSavingGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        var contribution = await dbContext.Set<SavingGoalContribution>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        contribution.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
