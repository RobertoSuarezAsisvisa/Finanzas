using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.RecurringRules.Commands;
using FinanzasMCP.Domain.Recurring;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.RecurringRules.Handlers;

public sealed class UpdateRecurringRuleHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<RecurringRuleSummary> Handle(UpdateRecurringRuleCommand command, CancellationToken cancellationToken = default)
    {
        var rule = await dbContext.Set<RecurringRule>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        rule.UpdateDetails(command.Name, command.Type, command.Amount, command.AccountId, command.CategoryId, command.Frequency, command.StartDate.ToUtcSafe(), command.EndDate?.ToUtcSafe(), command.NextDueDate.ToUtcSafe(), command.IsActive);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new RecurringRuleSummary(rule.Id, rule.Name, rule.Type, rule.Amount, rule.AccountId, rule.CategoryId, rule.Frequency, rule.StartDate, rule.EndDate, rule.NextDueDate, rule.IsActive);
    }
}
