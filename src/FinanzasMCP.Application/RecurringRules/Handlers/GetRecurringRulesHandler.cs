using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.RecurringRules.Queries;
using FinanzasMCP.Domain.Recurring;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.RecurringRules.Handlers;

public sealed class GetRecurringRulesHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<RecurringRuleSummary>> Handle(GetRecurringRulesQuery query, CancellationToken cancellationToken = default)
    {
        var rules = await dbContext.Set<RecurringRule>().AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);
        return rules.Select(x => new RecurringRuleSummary(x.Id, x.Name, x.Type, x.Amount, x.AccountId, x.CategoryId, x.Frequency, x.StartDate, x.EndDate, x.NextDueDate, x.IsActive)).ToArray();
    }
}
