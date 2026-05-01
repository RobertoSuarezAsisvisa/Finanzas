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
        var totalIncome = await dbContext.Set<Transaction>().Where(x => x.Type == TransactionType.Income).SumAsync(x => x.Amount, cancellationToken);
        var totalExpenses = await dbContext.Set<Transaction>().Where(x => x.Type == TransactionType.Expense).SumAsync(x => x.Amount, cancellationToken);
        var totalAssets = await dbContext.Accounts.SumAsync(x => x.Balance, cancellationToken);
        var totalDebts = await dbContext.Set<Debt>().Where(x => x.Status != DebtStatus.Paid).SumAsync(x => x.RemainingAmount, cancellationToken);
        var savingGoalsProgress = await dbContext.Set<SavingGoal>()
            .Select(x => (decimal?)(x.GoalAmount == 0 ? 0m : (x.CurrentAmount / x.GoalAmount) * 100m))
            .AverageAsync(cancellationToken) ?? 0m;

        var purchaseGoalsProgress = await dbContext.Set<PurchaseGoal>()
            .Select(x => (decimal?)(x.GoalPrice == 0 ? 0m : (x.SavedAmount / x.GoalPrice) * 100m))
            .AverageAsync(cancellationToken) ?? 0m;

        return new FinanceOverviewSummary(totalIncome, totalExpenses, totalIncome - totalExpenses, totalAssets, totalDebts, savingGoalsProgress, purchaseGoalsProgress);
    }
}
