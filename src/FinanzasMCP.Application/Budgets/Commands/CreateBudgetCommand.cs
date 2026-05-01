using FinanzasMCP.Domain.Budgets;

namespace FinanzasMCP.Application.Budgets.Commands;

public sealed record CreateBudgetCommand(string Name, Guid CategoryId, decimal LimitAmount, PeriodType PeriodType, BudgetValidityType ValidityType, DateTimeOffset? PeriodStart, DateTimeOffset? PeriodEnd);
