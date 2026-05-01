using FinanzasMCP.Application.Budgets.Commands;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Budgets;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Budgets.Handlers;

public sealed class UpdateBudgetHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<BudgetSummary> Handle(UpdateBudgetCommand command, CancellationToken cancellationToken = default)
    {
        var budget = await dbContext.Set<Budget>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        budget.UpdateDetails(command.Name, command.LimitAmount, command.PeriodType, command.ValidityType, command.PeriodStart, command.PeriodEnd, command.IsActive);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BudgetSummary(budget.Id, budget.Name, budget.CategoryId, budget.LimitAmount, budget.PeriodType, budget.ValidityType, budget.PeriodStart, budget.PeriodEnd, budget.IsActive);
    }
}
