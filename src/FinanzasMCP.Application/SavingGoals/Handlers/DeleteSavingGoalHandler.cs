using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class DeleteSavingGoalHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteSavingGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = await dbContext.Set<SavingGoal>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        goal.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
