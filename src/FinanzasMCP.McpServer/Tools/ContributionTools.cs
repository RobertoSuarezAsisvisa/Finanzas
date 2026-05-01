using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.PurchaseGoals.Commands;
using FinanzasMCP.Application.PurchaseGoals.Handlers;
using FinanzasMCP.Application.PurchaseGoals.Queries;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Application.SavingGoals.Handlers;
using FinanzasMCP.Application.SavingGoals.Queries;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class ContributionTools(
    AddSavingGoalContributionHandler addSavingGoalContributionHandler,
    UpdateSavingGoalContributionHandler updateSavingGoalContributionHandler,
    DeleteSavingGoalContributionHandler deleteSavingGoalContributionHandler,
    GetSavingGoalContributionsHandler getSavingGoalContributionsHandler,
    AddPurchaseGoalContributionHandler addPurchaseGoalContributionHandler,
    UpdatePurchaseGoalContributionHandler updatePurchaseGoalContributionHandler,
    DeletePurchaseGoalContributionHandler deletePurchaseGoalContributionHandler,
    GetPurchaseGoalContributionsHandler getPurchaseGoalContributionsHandler)
{
    [McpServerTool, System.ComponentModel.Description("Adds a contribution to a saving goal.")]
    public Task<SavingGoalSummary> AddSavingGoalContribution(Guid goalId, decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => addSavingGoalContributionHandler.Handle(new AddSavingGoalContributionCommand(goalId, amount, contributionDate, transactionId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists contributions for a saving goal.")]
    public Task<IReadOnlyList<SavingGoalContributionSummary>> ListSavingGoalContributions(Guid? goalId = null, CancellationToken cancellationToken = default)
        => getSavingGoalContributionsHandler.Handle(new GetSavingGoalContributionsQuery(goalId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a saving goal contribution.")]
    public Task UpdateSavingGoalContribution(Guid id, decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => updateSavingGoalContributionHandler.Handle(new UpdateSavingGoalContributionCommand(id, amount, contributionDate, transactionId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a saving goal contribution.")]
    public Task DeleteSavingGoalContribution(Guid id, CancellationToken cancellationToken = default)
        => deleteSavingGoalContributionHandler.Handle(new DeleteSavingGoalContributionCommand(id), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Adds a contribution to a purchase goal.")]
    public Task<PurchaseGoalSummary> AddPurchaseGoalContribution(Guid purchaseGoalId, decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => addPurchaseGoalContributionHandler.Handle(new AddPurchaseGoalContributionCommand(purchaseGoalId, amount, contributionDate, transactionId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists contributions for a purchase goal.")]
    public Task<IReadOnlyList<PurchaseGoalContributionSummary>> ListPurchaseGoalContributions(Guid? purchaseGoalId = null, CancellationToken cancellationToken = default)
        => getPurchaseGoalContributionsHandler.Handle(new GetPurchaseGoalContributionsQuery(purchaseGoalId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a purchase goal contribution.")]
    public Task UpdatePurchaseGoalContribution(Guid id, decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => updatePurchaseGoalContributionHandler.Handle(new UpdatePurchaseGoalContributionCommand(id, amount, contributionDate, transactionId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a purchase goal contribution.")]
    public Task DeletePurchaseGoalContribution(Guid id, CancellationToken cancellationToken = default)
        => deletePurchaseGoalContributionHandler.Handle(new DeletePurchaseGoalContributionCommand(id), cancellationToken);
}
