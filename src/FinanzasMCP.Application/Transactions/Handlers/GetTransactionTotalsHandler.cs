using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Transactions.Queries;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Transactions.Handlers;

public sealed class GetTransactionTotalsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<TransactionTotalsSummary> Handle(GetTransactionTotalsQuery query, CancellationToken cancellationToken = default)
    {
        var transactions = dbContext.Set<Transaction>()
            .AsNoTracking()
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

        var totals = await transactions
            .GroupBy(_ => 1)
            .Select(group => new
            {
                TotalIncome = group.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount),
                TotalExpenses = group.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount),
                TotalTransfers = group.Where(x => x.Type == TransactionType.Transfer).Sum(x => x.Amount),
                TransactionCount = group.Count(),
                IncomeCount = group.Count(x => x.Type == TransactionType.Income),
                ExpenseCount = group.Count(x => x.Type == TransactionType.Expense),
                TransferCount = group.Count(x => x.Type == TransactionType.Transfer)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (totals is null)
        {
            return new TransactionTotalsSummary(0m, 0m, 0m, 0m, 0, 0, 0, 0, 0m);
        }

        var averageExpense = totals.ExpenseCount > 0 ? totals.TotalExpenses / totals.ExpenseCount : 0m;

        return new TransactionTotalsSummary(
            totals.TotalIncome,
            totals.TotalExpenses,
            totals.TotalIncome - totals.TotalExpenses,
            totals.TotalTransfers,
            totals.TransactionCount,
            totals.IncomeCount,
            totals.ExpenseCount,
            totals.TransferCount,
            averageExpense);
    }
}
