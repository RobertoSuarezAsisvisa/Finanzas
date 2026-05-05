using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.PurchaseGoals.Queries;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.PurchaseGoals.Handlers;

public sealed class GetPurchaseGoalContributionsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<PurchaseGoalContributionSummary>> Handle(GetPurchaseGoalContributionsQuery query, CancellationToken cancellationToken = default)
    {
        var contributions = dbContext.Set<PurchaseGoalContribution>().AsNoTracking().AsQueryable();
        if (query.PurchaseGoalId is not null)
        {
            contributions = contributions.Where(x => x.PurchaseGoalId == query.PurchaseGoalId);
        }

        var list = await contributions.OrderByDescending(x => x.ContributionDate).ToListAsync(cancellationToken);
        var transactionIds = list
            .Where(x => x.TransactionId is not null)
            .Select(x => x.TransactionId!.Value)
            .ToArray();
        var transactionAccounts = await dbContext.Set<Transaction>()
            .AsNoTracking()
            .Where(x => transactionIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => (Guid?)x.AccountId, cancellationToken);

        return list
            .Select(x => new PurchaseGoalContributionSummary(
                x.Id,
                x.PurchaseGoalId,
                x.TransactionId,
                x.TransactionId is not null && transactionAccounts.TryGetValue(x.TransactionId.Value, out var accountId) ? accountId : null,
                x.Amount,
                x.ContributionDate))
            .ToArray();
    }
}
