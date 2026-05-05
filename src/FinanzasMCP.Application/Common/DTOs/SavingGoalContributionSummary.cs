namespace FinanzasMCP.Application.Common.DTOs;

public sealed record SavingGoalContributionSummary(Guid Id, Guid GoalId, Guid? TransactionId, Guid? AccountId, decimal Amount, DateTimeOffset ContributionDate);
