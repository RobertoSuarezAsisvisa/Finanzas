using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.RecurringRules.Commands;
using FinanzasMCP.Domain.Recurring;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.RecurringRules.Handlers;

public sealed class DeleteRecurringRuleHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteRecurringRuleCommand command, CancellationToken cancellationToken = default)
    {
        var rule = await dbContext.Set<RecurringRule>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        rule.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
