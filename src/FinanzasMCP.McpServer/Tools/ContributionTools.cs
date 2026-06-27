using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Goals.Handlers;
using FinanzasMCP.Application.Goals.Queries;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class ContributionTools(
    AddFinancialGoalContributionHandler addContributionHandler,
    UpdateFinancialGoalContributionHandler updateContributionHandler,
    DeleteFinancialGoalContributionHandler deleteContributionHandler,
    GetFinancialGoalContributionsHandler getContributionsHandler)
{
    [McpServerTool, System.ComponentModel.Description("Deprecated. Adds a contribution to a saving goal.")]
    public Task<FinancialGoalSummary> AddSavingGoalContribution(Guid goalId, decimal amount, DateTimeOffset contributionDate, Guid? accountId = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => addContributionHandler.Handle(new AddFinancialGoalContributionCommand(goalId, amount, contributionDate, transactionId, accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Lists contributions for a saving goal.")]
    public Task<IReadOnlyList<FinancialGoalContributionSummary>> ListSavingGoalContributions(Guid? goalId = null, CancellationToken cancellationToken = default)
        => getContributionsHandler.Handle(new GetFinancialGoalContributionsQuery(goalId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Updates a saving goal contribution.")]
    public Task UpdateSavingGoalContribution(Guid id, decimal amount, DateTimeOffset contributionDate, Guid? accountId = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => updateContributionHandler.Handle(new UpdateFinancialGoalContributionCommand(id, amount, contributionDate, transactionId, accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Logically deletes a saving goal contribution.")]
    public Task DeleteSavingGoalContribution(Guid id, CancellationToken cancellationToken = default)
        => deleteContributionHandler.Handle(new DeleteFinancialGoalContributionCommand(id), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Adds a contribution to a purchase goal.")]
    public Task<FinancialGoalSummary> AddPurchaseGoalContribution(Guid purchaseGoalId, decimal amount, DateTimeOffset contributionDate, Guid? accountId = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => addContributionHandler.Handle(new AddFinancialGoalContributionCommand(purchaseGoalId, amount, contributionDate, transactionId, accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Lists contributions for a purchase goal.")]
    public Task<IReadOnlyList<FinancialGoalContributionSummary>> ListPurchaseGoalContributions(Guid? purchaseGoalId = null, CancellationToken cancellationToken = default)
        => getContributionsHandler.Handle(new GetFinancialGoalContributionsQuery(purchaseGoalId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Updates a purchase goal contribution.")]
    public Task UpdatePurchaseGoalContribution(Guid id, decimal amount, DateTimeOffset contributionDate, Guid? accountId = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => updateContributionHandler.Handle(new UpdateFinancialGoalContributionCommand(id, amount, contributionDate, transactionId, accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Logically deletes a purchase goal contribution.")]
    public Task DeletePurchaseGoalContribution(Guid id, CancellationToken cancellationToken = default)
        => deleteContributionHandler.Handle(new DeleteFinancialGoalContributionCommand(id), cancellationToken);
}
