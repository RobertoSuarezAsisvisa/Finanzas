using FinanzasMCP.Application.Budgets.Commands;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Budgets;

namespace FinanzasMCP.Application.Budgets.Handlers;

public sealed class CreateBudgetHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<BudgetSummary> Handle(CreateBudgetCommand command, CancellationToken cancellationToken = default)
    {
        var budget = Budget.Create(command.Name, command.LimitAmount, command.PeriodType, command.ValidityType, command.PeriodStart, command.PeriodEnd);
        dbContext.Set<Budget>().Add(budget);
        await dbContext.SaveChangesAsync(cancellationToken);
        return BudgetUsageCalculator.ToSummary(budget, 0m, 0, BudgetUsageCalculator.CurrentPeriod(budget, DateTimeOffset.UtcNow));
    }
}
