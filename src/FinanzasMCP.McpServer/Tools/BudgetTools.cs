using FinanzasMCP.Application.Budgets.Commands;
using FinanzasMCP.Application.Budgets.Handlers;
using FinanzasMCP.Application.Budgets.Queries;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Domain.Budgets;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class BudgetTools(
    CreateBudgetHandler createBudgetHandler,
    GetBudgetsHandler getBudgetsHandler,
    UpdateBudgetHandler updateBudgetHandler,
    DeleteBudgetHandler deleteBudgetHandler)
{
    [McpServerTool, System.ComponentModel.Description("Creates a budget by category and validity mode.")]
    public Task<BudgetSummary> CreateBudget(string name, Guid categoryId, decimal limitAmount, PeriodType periodType, BudgetValidityType validityType, DateTimeOffset? periodStart = null, DateTimeOffset? periodEnd = null, CancellationToken cancellationToken = default)
        => createBudgetHandler.Handle(new CreateBudgetCommand(name, categoryId, limitAmount, periodType, validityType, periodStart, periodEnd), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists budgets.")]
    public Task<IReadOnlyList<BudgetSummary>> ListBudgets(CancellationToken cancellationToken = default)
        => getBudgetsHandler.Handle(new GetBudgetsQuery(), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a budget.")]
    public Task<BudgetSummary> UpdateBudget(Guid id, string name, decimal limitAmount, PeriodType periodType, BudgetValidityType validityType, DateTimeOffset? periodStart = null, DateTimeOffset? periodEnd = null, bool isActive = true, CancellationToken cancellationToken = default)
        => updateBudgetHandler.Handle(new UpdateBudgetCommand(id, name, limitAmount, periodType, validityType, periodStart, periodEnd, isActive), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a budget.")]
    public Task DeleteBudget(Guid id, CancellationToken cancellationToken = default)
        => deleteBudgetHandler.Handle(new DeleteBudgetCommand(id), cancellationToken);
}
