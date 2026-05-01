using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record SavingGoalSummary(Guid Id, string Name, decimal TargetAmount, decimal CurrentAmount, decimal? SuggestedMonthlyContribution, Guid? AccountId, DateTimeOffset? TargetDate, SavingGoalStatus Status);
