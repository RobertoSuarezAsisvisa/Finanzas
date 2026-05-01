using FinanzasMCP.Domain.Budgets;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record BudgetSummary(Guid Id, string Name, Guid CategoryId, decimal LimitAmount, PeriodType PeriodType, BudgetValidityType ValidityType, DateTimeOffset? PeriodStart, DateTimeOffset? PeriodEnd, bool IsActive);
