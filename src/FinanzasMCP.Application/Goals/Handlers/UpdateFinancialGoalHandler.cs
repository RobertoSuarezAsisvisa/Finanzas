using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Goals.Handlers;

public sealed class UpdateFinancialGoalHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<FinancialGoalSummary> Handle(UpdateFinancialGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = await dbContext.Set<FinancialGoal>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        goal.UpdateDetails(command.Name, command.TargetAmount, command.Type, command.Description, command.Priority, command.Url, command.AccountId, command.TargetDate, command.Status, command.CompletedAt);
        await dbContext.SaveChangesAsync(cancellationToken);
        return FinancialGoalMapping.ToSummary(goal);
    }
}
