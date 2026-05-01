using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.RecurringRules.Commands;
using FinanzasMCP.Application.RecurringRules.Handlers;
using FinanzasMCP.Application.RecurringRules.Queries;
using FinanzasMCP.Domain.Recurring;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class RecurringRuleTools(
    CreateRecurringRuleHandler createRecurringRuleHandler,
    GetRecurringRulesHandler getRecurringRulesHandler,
    UpdateRecurringRuleHandler updateRecurringRuleHandler,
    DeleteRecurringRuleHandler deleteRecurringRuleHandler)
{
    [McpServerTool, System.ComponentModel.Description("Creates a recurring rule.")]
    public Task<RecurringRuleSummary> CreateRecurringRule(string name, RecurringType type, decimal amount, Guid accountId, Guid? categoryId, RecurringFrequency frequency, DateTimeOffset startDate, DateTimeOffset? endDate, DateTimeOffset nextDueDate, CancellationToken cancellationToken = default)
        => createRecurringRuleHandler.Handle(new CreateRecurringRuleCommand(name, type, amount, accountId, categoryId, frequency, startDate, endDate, nextDueDate), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists recurring rules.")]
    public Task<IReadOnlyList<RecurringRuleSummary>> ListRecurringRules(CancellationToken cancellationToken = default)
        => getRecurringRulesHandler.Handle(new GetRecurringRulesQuery(), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a recurring rule.")]
    public Task<RecurringRuleSummary> UpdateRecurringRule(Guid id, string name, RecurringType type, decimal amount, Guid accountId, Guid? categoryId, RecurringFrequency frequency, DateTimeOffset startDate, DateTimeOffset? endDate, DateTimeOffset nextDueDate, bool isActive, CancellationToken cancellationToken = default)
        => updateRecurringRuleHandler.Handle(new UpdateRecurringRuleCommand(id, name, type, amount, accountId, categoryId, frequency, startDate, endDate, nextDueDate, isActive), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a recurring rule.")]
    public Task DeleteRecurringRule(Guid id, CancellationToken cancellationToken = default)
        => deleteRecurringRuleHandler.Handle(new DeleteRecurringRuleCommand(id), cancellationToken);
}
