using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Queries;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class GetDebtsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<DebtSummary>> Handle(GetDebtsQuery query, CancellationToken cancellationToken = default)
    {
        var debts = await dbContext.Set<Debt>()
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return debts
            .Select(x => new DebtSummary(x.Id, x.Type, x.ContactName, x.OriginalAmount, x.RemainingAmount, x.Currency, x.DueDate, x.AccountId, x.Status, x.Notes))
            .ToArray();
    }
}
