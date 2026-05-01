using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.RecurringRules.Commands;
using FinanzasMCP.Domain.Recurring;

namespace FinanzasMCP.Application.RecurringRules.Handlers;

public sealed class CreateRecurringRuleHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<RecurringRuleSummary> Handle(CreateRecurringRuleCommand command, CancellationToken cancellationToken = default)
    {
        var rule = RecurringRule.Create(command.Name, command.Type, command.Amount, command.AccountId, command.CategoryId, command.Frequency, command.StartDate.ToUtcSafe(), command.EndDate?.ToUtcSafe(), command.NextDueDate.ToUtcSafe());
        dbContext.Set<RecurringRule>().Add(rule);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RecurringRuleSummary(rule.Id, rule.Name, rule.Type, rule.Amount, rule.AccountId, rule.CategoryId, rule.Frequency, rule.StartDate, rule.EndDate, rule.NextDueDate, rule.IsActive);
    }
}
