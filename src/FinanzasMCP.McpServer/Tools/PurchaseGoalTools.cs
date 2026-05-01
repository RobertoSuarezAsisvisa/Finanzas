using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.PurchaseGoals.Commands;
using FinanzasMCP.Application.PurchaseGoals.Handlers;
using FinanzasMCP.Application.PurchaseGoals.Queries;
using FinanzasMCP.Domain.Goals;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class PurchaseGoalTools(
    GetPurchaseGoalsHandler getPurchaseGoalsHandler,
    CreatePurchaseGoalHandler createPurchaseGoalHandler,
    AddPurchaseGoalContributionHandler addPurchaseGoalContributionHandler,
    UpdatePurchaseGoalHandler updatePurchaseGoalHandler,
    DeletePurchaseGoalHandler deletePurchaseGoalHandler)
{
    [McpServerTool, System.ComponentModel.Description("Lists purchase goals.")]
    public Task<IReadOnlyList<PurchaseGoalSummary>> ListPurchaseGoals(CancellationToken cancellationToken = default)
        => getPurchaseGoalsHandler.Handle(new GetPurchaseGoalsQuery(), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Creates a purchase goal.")]
    public Task<PurchaseGoalSummary> CreatePurchaseGoal(string name, decimal targetPrice, string? description = null, int priority = 1, string? url = null, Guid? accountId = null, DateTimeOffset? targetDate = null, CancellationToken cancellationToken = default)
        => createPurchaseGoalHandler.Handle(new CreatePurchaseGoalCommand(name, targetPrice, description, priority, url, accountId, targetDate), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Adds a contribution to a purchase goal.")]
    public Task<PurchaseGoalSummary> AddContribution(Guid purchaseGoalId, decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => addPurchaseGoalContributionHandler.Handle(new AddPurchaseGoalContributionCommand(purchaseGoalId, amount, contributionDate, transactionId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a purchase goal.")]
    public Task<PurchaseGoalSummary> UpdatePurchaseGoal(Guid id, string name, decimal targetPrice, string? description = null, int priority = 1, string? url = null, Guid? accountId = null, DateTimeOffset? targetDate = null, PurchaseGoalStatus? status = null, DateTimeOffset? purchasedAt = null, CancellationToken cancellationToken = default)
        => updatePurchaseGoalHandler.Handle(new UpdatePurchaseGoalCommand(id, name, targetPrice, description, priority, url, accountId, targetDate, status, purchasedAt), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a purchase goal.")]
    public Task DeletePurchaseGoal(Guid id, CancellationToken cancellationToken = default)
        => deletePurchaseGoalHandler.Handle(new DeletePurchaseGoalCommand(id), cancellationToken);
}
