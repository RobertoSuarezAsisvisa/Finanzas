using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.SavingGoals.Queries;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.SavingGoals.Handlers;

public sealed class GetSavingGoalContributionsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<SavingGoalContributionSummary>> Handle(GetSavingGoalContributionsQuery query, CancellationToken cancellationToken = default)
    {
        var contributions = dbContext.Set<SavingGoalContribution>().AsNoTracking().AsQueryable();
        if (query.GoalId is not null)
        {
            contributions = contributions.Where(x => x.GoalId == query.GoalId);
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
            .Select(x => new SavingGoalContributionSummary(
                x.Id,
                x.GoalId,
                x.TransactionId,
                x.TransactionId is not null && transactionAccounts.TryGetValue(x.TransactionId.Value, out var accountId) ? accountId : null,
                x.Amount,
                x.ContributionDate))
            .ToArray();
    }
}
