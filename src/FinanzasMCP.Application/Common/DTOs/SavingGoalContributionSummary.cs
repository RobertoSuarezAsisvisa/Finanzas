namespace FinanzasMCP.Application.Common.DTOs;

public sealed record SavingGoalContributionSummary(Guid Id, Guid GoalId, Guid? TransactionId, decimal Amount, DateTimeOffset ContributionDate);
