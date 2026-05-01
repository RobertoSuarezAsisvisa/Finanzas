using FinanzasMCP.Application.Budgets.Commands;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Budgets;

namespace FinanzasMCP.Application.Budgets.Handlers;

public sealed class CreateBudgetHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<BudgetSummary> Handle(CreateBudgetCommand command, CancellationToken cancellationToken = default)
    {
        var budget = Budget.Create(command.Name, command.CategoryId, command.LimitAmount, command.PeriodType, command.ValidityType, command.PeriodStart, command.PeriodEnd);
        dbContext.Set<Budget>().Add(budget);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BudgetSummary(budget.Id, budget.Name, budget.CategoryId, budget.LimitAmount, budget.PeriodType, budget.ValidityType, budget.PeriodStart, budget.PeriodEnd, budget.IsActive);
    }
}
