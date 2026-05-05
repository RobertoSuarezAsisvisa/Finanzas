namespace FinanzasMCP.Application.Common.DTOs;

public sealed record PurchaseGoalContributionSummary(Guid Id, Guid PurchaseGoalId, Guid? TransactionId, Guid? AccountId, decimal Amount, DateTimeOffset ContributionDate);
