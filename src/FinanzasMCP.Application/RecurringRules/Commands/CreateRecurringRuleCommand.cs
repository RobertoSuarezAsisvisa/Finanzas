using FinanzasMCP.Domain.Recurring;

namespace FinanzasMCP.Application.RecurringRules.Commands;

public sealed record CreateRecurringRuleCommand(string Name, RecurringType Type, decimal Amount, Guid AccountId, Guid? CategoryId, RecurringFrequency Frequency, DateTimeOffset StartDate, DateTimeOffset? EndDate, DateTimeOffset NextDueDate);
