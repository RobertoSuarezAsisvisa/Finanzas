using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Goals.Handlers;
using FinanzasMCP.Application.Goals.Queries;
using FinanzasMCP.Domain.Goals;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class PurchaseGoalTools(
    GetFinancialGoalsHandler getGoalsHandler,
    CreateFinancialGoalHandler createGoalHandler,
    AddFinancialGoalContributionHandler addContributionHandler,
    UpdateFinancialGoalHandler updateGoalHandler,
    DeleteFinancialGoalHandler deleteGoalHandler)
{
    [McpServerTool, System.ComponentModel.Description("Deprecated. Lists purchase financial goals.")]
    public Task<IReadOnlyList<FinancialGoalSummary>> ListPurchaseGoals(CancellationToken cancellationToken = default)
        => getGoalsHandler.Handle(new GetFinancialGoalsQuery(FinancialGoalType.Purchase), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Creates a purchase financial goal.")]
    public Task<FinancialGoalSummary> CreatePurchaseGoal(string name, decimal targetPrice, string? description = null, int priority = 1, string? url = null, Guid? accountId = null, DateTimeOffset? targetDate = null, CancellationToken cancellationToken = default)
        => createGoalHandler.Handle(new CreateFinancialGoalCommand(name, targetPrice, FinancialGoalType.Purchase, description, priority, url, accountId, targetDate), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Adds a contribution to a purchase financial goal.")]
    public Task<FinancialGoalSummary> AddContribution(Guid purchaseGoalId, decimal amount, DateTimeOffset contributionDate, Guid? accountId = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => addContributionHandler.Handle(new AddFinancialGoalContributionCommand(purchaseGoalId, amount, contributionDate, transactionId, accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Updates a purchase financial goal.")]
    public Task<FinancialGoalSummary> UpdatePurchaseGoal(Guid id, string name, decimal targetPrice, string? description = null, int priority = 1, string? url = null, Guid? accountId = null, DateTimeOffset? targetDate = null, PurchaseGoalStatus? status = null, DateTimeOffset? purchasedAt = null, CancellationToken cancellationToken = default)
        => updateGoalHandler.Handle(new UpdateFinancialGoalCommand(id, name, targetPrice, FinancialGoalType.Purchase, description, priority, url, accountId, targetDate, MapStatus(status), purchasedAt), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Logically deletes a purchase financial goal.")]
    public Task DeletePurchaseGoal(Guid id, CancellationToken cancellationToken = default)
        => deleteGoalHandler.Handle(new DeleteFinancialGoalCommand(id), cancellationToken);

    private static FinancialGoalStatus? MapStatus(PurchaseGoalStatus? status)
        => status switch
        {
            null => null,
            PurchaseGoalStatus.Saving => FinancialGoalStatus.InProgress,
            PurchaseGoalStatus.Ready => FinancialGoalStatus.Ready,
            PurchaseGoalStatus.Purchased => FinancialGoalStatus.Completed,
            PurchaseGoalStatus.Cancelled => FinancialGoalStatus.Cancelled,
            _ => throw new InvalidOperationException("Unsupported purchase goal status.")
        };
}
