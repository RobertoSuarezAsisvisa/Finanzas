using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Goals.Handlers;
using FinanzasMCP.Application.Goals.Queries;
using FinanzasMCP.Domain.Goals;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class GoalTools(
    GetFinancialGoalsHandler getGoalsHandler,
    CreateFinancialGoalHandler createGoalHandler,
    AddFinancialGoalContributionHandler addContributionHandler,
    GetFinancialGoalContributionsHandler getContributionsHandler,
    UpdateFinancialGoalHandler updateGoalHandler,
    UpdateFinancialGoalContributionHandler updateContributionHandler,
    DeleteFinancialGoalContributionHandler deleteContributionHandler,
    DeleteFinancialGoalHandler deleteGoalHandler)
{
    [McpServerTool, System.ComponentModel.Description("Lists financial goals.")]
    public Task<IReadOnlyList<FinancialGoalSummary>> ListGoals(FinancialGoalType? type = null, CancellationToken cancellationToken = default)
        => getGoalsHandler.Handle(new GetFinancialGoalsQuery(type), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Creates a financial goal.")]
    public Task<FinancialGoalSummary> CreateGoal(string name, decimal targetAmount, FinancialGoalType type = FinancialGoalType.Saving, string? description = null, int priority = 1, string? url = null, Guid? accountId = null, DateTimeOffset? targetDate = null, CancellationToken cancellationToken = default)
        => createGoalHandler.Handle(new CreateFinancialGoalCommand(name, targetAmount, type, description, priority, url, accountId, targetDate), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a financial goal.")]
    public Task<FinancialGoalSummary> UpdateGoal(Guid id, string name, decimal targetAmount, FinancialGoalType type = FinancialGoalType.Saving, string? description = null, int priority = 1, string? url = null, Guid? accountId = null, DateTimeOffset? targetDate = null, FinancialGoalStatus? status = null, DateTimeOffset? completedAt = null, CancellationToken cancellationToken = default)
        => updateGoalHandler.Handle(new UpdateFinancialGoalCommand(id, name, targetAmount, type, description, priority, url, accountId, targetDate, status, completedAt), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a financial goal.")]
    public Task DeleteGoal(Guid id, CancellationToken cancellationToken = default)
        => deleteGoalHandler.Handle(new DeleteFinancialGoalCommand(id), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Adds a contribution to a financial goal.")]
    public Task<FinancialGoalSummary> AddGoalContribution(Guid goalId, decimal amount, DateTimeOffset contributionDate, Guid? accountId = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => addContributionHandler.Handle(new AddFinancialGoalContributionCommand(goalId, amount, contributionDate, transactionId, accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists contributions for a financial goal.")]
    public Task<IReadOnlyList<FinancialGoalContributionSummary>> ListGoalContributions(Guid? goalId = null, CancellationToken cancellationToken = default)
        => getContributionsHandler.Handle(new GetFinancialGoalContributionsQuery(goalId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a financial goal contribution.")]
    public Task UpdateGoalContribution(Guid id, decimal amount, DateTimeOffset contributionDate, Guid? accountId = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => updateContributionHandler.Handle(new UpdateFinancialGoalContributionCommand(id, amount, contributionDate, transactionId, accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a financial goal contribution.")]
    public Task DeleteGoalContribution(Guid id, CancellationToken cancellationToken = default)
        => deleteContributionHandler.Handle(new DeleteFinancialGoalContributionCommand(id), cancellationToken);
}
