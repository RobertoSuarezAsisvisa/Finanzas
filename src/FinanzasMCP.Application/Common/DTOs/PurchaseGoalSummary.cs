using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record PurchaseGoalSummary(Guid Id, string Name, decimal TargetPrice, decimal SavedAmount, decimal? SuggestedMonthlyContribution, int Priority, string? Url, Guid? AccountId, DateTimeOffset? TargetDate, PurchaseGoalStatus Status);
