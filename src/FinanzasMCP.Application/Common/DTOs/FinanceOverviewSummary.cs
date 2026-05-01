namespace FinanzasMCP.Application.Common.DTOs;

public sealed record FinanceOverviewSummary(
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal NetBalance,
    decimal TotalAssets,
    decimal TotalDebts,
    decimal SavingGoalsProgress,
    decimal PurchaseGoalsProgress);
