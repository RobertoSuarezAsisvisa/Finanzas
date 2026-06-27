using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Reports.Queries;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Debts;
using FinanzasMCP.Domain.Goals;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Reports.Handlers;

public sealed class GetFinanceOverviewHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<FinanceOverviewSummary> Handle(GetFinanceOverviewQuery query, CancellationToken cancellationToken = default)
    {
        var transactions = dbContext.Set<Transaction>().AsNoTracking().AsQueryable();

        if (query.DateFrom is not null)
        {
            transactions = transactions.Where(x => x.TransactionDate >= query.DateFrom);
        }

        if (query.DateTo is not null)
        {
            transactions = transactions.Where(x => x.TransactionDate <= query.DateTo);
        }

        var totalIncome = await transactions.Where(x => x.Type == TransactionType.Income).SumAsync(x => x.Amount, cancellationToken);
        var totalExpenses = await transactions.Where(x => x.Type == TransactionType.Expense).SumAsync(x => x.Amount, cancellationToken);
        var totalAssets = await dbContext.Accounts.Where(x => x.IsActive).SumAsync(x => x.Balance, cancellationToken);
        var totalDebts = await dbContext.Set<Debt>()
            .Where(x => x.Type == DebtType.Payable && x.Status == DebtStatus.Active)
            .SumAsync(x => x.RemainingAmount, cancellationToken);
        var goalsProgress = await dbContext.Set<FinancialGoal>()
            .Select(x => (decimal?)(x.TargetAmount == 0 ? 0m : (x.CurrentAmount / x.TargetAmount) * 100m))
            .AverageAsync(cancellationToken) ?? 0m;

        return new FinanceOverviewSummary(totalIncome, totalExpenses, totalIncome - totalExpenses, totalAssets, totalDebts, goalsProgress);
    }
}
