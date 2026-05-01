using FinanzasMCP.Domain.Budgets;

namespace FinanzasMCP.Application.Budgets.Commands;

public sealed record UpdateBudgetCommand(Guid Id, string Name, decimal LimitAmount, PeriodType PeriodType, BudgetValidityType ValidityType, DateTimeOffset? PeriodStart, DateTimeOffset? PeriodEnd, bool IsActive);
