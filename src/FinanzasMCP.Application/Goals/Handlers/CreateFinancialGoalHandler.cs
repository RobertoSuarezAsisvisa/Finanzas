using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.Goals.Handlers;

public sealed class CreateFinancialGoalHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<FinancialGoalSummary> Handle(CreateFinancialGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = FinancialGoal.Create(command.Name, command.TargetAmount, command.Type, command.Description, command.Priority, command.Url, command.AccountId, command.TargetDate);
        dbContext.Set<FinancialGoal>().Add(goal);
        await dbContext.SaveChangesAsync(cancellationToken);
        return FinancialGoalMapping.ToSummary(goal);
    }
}
