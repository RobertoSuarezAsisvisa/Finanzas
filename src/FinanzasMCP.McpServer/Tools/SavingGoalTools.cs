using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Goals.Handlers;
using FinanzasMCP.Application.Goals.Queries;
using FinanzasMCP.Domain.Goals;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class SavingGoalTools(
    GetFinancialGoalsHandler getGoalsHandler,
    CreateFinancialGoalHandler createGoalHandler,
    AddFinancialGoalContributionHandler addContributionHandler,
    UpdateFinancialGoalHandler updateGoalHandler,
    DeleteFinancialGoalHandler deleteGoalHandler)
{
    [McpServerTool, System.ComponentModel.Description("Deprecated. Lists saving financial goals.")]
    public Task<IReadOnlyList<FinancialGoalSummary>> ListSavingGoals(CancellationToken cancellationToken = default)
        => getGoalsHandler.Handle(new GetFinancialGoalsQuery(FinancialGoalType.Saving), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Creates a saving financial goal.")]
    public Task<FinancialGoalSummary> CreateSavingGoal(string name, decimal targetAmount, Guid? accountId = null, DateTimeOffset? targetDate = null, CancellationToken cancellationToken = default)
        => createGoalHandler.Handle(new CreateFinancialGoalCommand(name, targetAmount, FinancialGoalType.Saving, AccountId: accountId, TargetDate: targetDate), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Adds a contribution to a saving financial goal.")]
    public Task<FinancialGoalSummary> AddContribution(Guid goalId, decimal amount, DateTimeOffset contributionDate, Guid? accountId = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => addContributionHandler.Handle(new AddFinancialGoalContributionCommand(goalId, amount, contributionDate, transactionId, accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Updates a saving financial goal.")]
    public Task<FinancialGoalSummary> UpdateSavingGoal(Guid id, string name, decimal targetAmount, Guid? accountId = null, DateTimeOffset? targetDate = null, SavingGoalStatus? status = null, CancellationToken cancellationToken = default)
        => updateGoalHandler.Handle(new UpdateFinancialGoalCommand(id, name, targetAmount, FinancialGoalType.Saving, AccountId: accountId, TargetDate: targetDate, Status: MapStatus(status)), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Deprecated. Logically deletes a saving financial goal.")]
    public Task DeleteSavingGoal(Guid id, CancellationToken cancellationToken = default)
        => deleteGoalHandler.Handle(new DeleteFinancialGoalCommand(id), cancellationToken);

    private static FinancialGoalStatus? MapStatus(SavingGoalStatus? status)
        => status switch
        {
            null => null,
            SavingGoalStatus.InProgress => FinancialGoalStatus.InProgress,
            SavingGoalStatus.Completed => FinancialGoalStatus.Completed,
            SavingGoalStatus.Cancelled => FinancialGoalStatus.Cancelled,
            _ => throw new InvalidOperationException("Unsupported saving goal status.")
        };
}
