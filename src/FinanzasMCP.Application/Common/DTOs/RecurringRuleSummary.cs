using FinanzasMCP.Domain.Recurring;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record RecurringRuleSummary(Guid Id, string Name, RecurringType Type, decimal Amount, Guid AccountId, Guid? CategoryId, RecurringFrequency Frequency, DateTimeOffset StartDate, DateTimeOffset? EndDate, DateTimeOffset NextDueDate, bool IsActive);
