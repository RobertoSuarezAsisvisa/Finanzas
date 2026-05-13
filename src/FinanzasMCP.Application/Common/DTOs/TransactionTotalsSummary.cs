namespace FinanzasMCP.Application.Common.DTOs;

public sealed record TransactionTotalsSummary(
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal NetBalance,
    decimal TotalTransfers,
    int TransactionCount,
    int IncomeCount,
    int ExpenseCount,
    int TransferCount,
    decimal AverageExpense);
