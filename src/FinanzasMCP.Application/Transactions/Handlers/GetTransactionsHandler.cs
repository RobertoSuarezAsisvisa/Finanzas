using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Transactions.Queries;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Transactions.Handlers;

public sealed class GetTransactionsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<PagedResult<TransactionSummary>> Handle(GetTransactionsQuery query, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var transactions = dbContext.Set<Transaction>()
            .AsNoTracking()
            .Include(x => x.Tags)
            .AsQueryable();

        if (query.AccountId is not null)
        {
            transactions = transactions.Where(x => x.AccountId == query.AccountId || x.ToAccountId == query.AccountId);
        }

        if (query.Type is not null)
        {
            transactions = transactions.Where(x => x.Type == query.Type);
        }

        if (query.CategoryId is not null)
        {
            transactions = transactions.Where(x => x.CategoryId == query.CategoryId);
        }

        if (query.DateFrom is not null)
        {
            transactions = transactions.Where(x => x.TransactionDate >= query.DateFrom);
        }

        if (query.DateTo is not null)
        {
            transactions = transactions.Where(x => x.TransactionDate <= query.DateTo);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            transactions = transactions.Where(x =>
                (x.Description != null && x.Description.ToLower().Contains(search)) ||
                (x.Reference != null && x.Reference.ToLower().Contains(search)));
        }

        var totalCount = await transactions.CountAsync(cancellationToken);
        var list = await transactions
            .OrderByDescending(x => x.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = list.Select(x => new TransactionSummary(
            x.Id,
            x.Type,
            x.Amount,
            x.Currency,
            x.AccountId,
            x.ToAccountId,
            x.CategoryId,
            x.Description,
            x.Reference,
            x.TransactionDate,
            x.Tags.Select(tag => tag.TagId).ToArray())).ToArray();

        return new PagedResult<TransactionSummary>(items, totalCount, page, pageSize);
    }
}
