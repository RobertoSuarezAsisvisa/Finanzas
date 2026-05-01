using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Transactions.Queries;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Transactions.Handlers;

public sealed class GetTransactionsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<TransactionSummary>> Handle(GetTransactionsQuery query, CancellationToken cancellationToken = default)
    {
        var transactions = dbContext.Set<Transaction>().AsNoTracking().AsQueryable();
        if (query.AccountId is not null)
        {
            transactions = transactions.Where(x => x.AccountId == query.AccountId || x.ToAccountId == query.AccountId);
        }

        var list = await transactions.OrderByDescending(x => x.TransactionDate).ToListAsync(cancellationToken);
        return list.Select(x => new TransactionSummary(x.Id, x.Type, x.Amount, x.Currency, x.AccountId, x.ToAccountId, x.CategoryId, x.Description, x.Reference, x.TransactionDate)).ToArray();
    }
}
