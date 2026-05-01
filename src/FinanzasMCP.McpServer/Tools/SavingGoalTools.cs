using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Application.SavingGoals.Handlers;
using FinanzasMCP.Application.SavingGoals.Queries;
using FinanzasMCP.Domain.Goals;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class SavingGoalTools(
    GetSavingGoalsHandler getSavingGoalsHandler,
    CreateSavingGoalHandler createSavingGoalHandler,
    AddSavingGoalContributionHandler addSavingGoalContributionHandler,
    UpdateSavingGoalHandler updateSavingGoalHandler,
    DeleteSavingGoalHandler deleteSavingGoalHandler)
{
    [McpServerTool, System.ComponentModel.Description("Lists saving goals.")]
    public Task<IReadOnlyList<SavingGoalSummary>> ListSavingGoals(CancellationToken cancellationToken = default)
        => getSavingGoalsHandler.Handle(new GetSavingGoalsQuery(), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Creates a saving goal.")]
    public Task<SavingGoalSummary> CreateSavingGoal(string name, decimal targetAmount, Guid? accountId = null, DateTimeOffset? targetDate = null, CancellationToken cancellationToken = default)
        => createSavingGoalHandler.Handle(new CreateSavingGoalCommand(name, targetAmount, accountId, targetDate), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Adds a contribution to a saving goal.")]
    public Task<SavingGoalSummary> AddContribution(Guid goalId, decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => addSavingGoalContributionHandler.Handle(new AddSavingGoalContributionCommand(goalId, amount, contributionDate, transactionId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a saving goal.")]
    public Task<SavingGoalSummary> UpdateSavingGoal(Guid id, string name, decimal targetAmount, Guid? accountId = null, DateTimeOffset? targetDate = null, SavingGoalStatus? status = null, CancellationToken cancellationToken = default)
        => updateSavingGoalHandler.Handle(new UpdateSavingGoalCommand(id, name, targetAmount, accountId, targetDate, status), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a saving goal.")]
    public Task DeleteSavingGoal(Guid id, CancellationToken cancellationToken = default)
        => deleteSavingGoalHandler.Handle(new DeleteSavingGoalCommand(id), cancellationToken);
}
