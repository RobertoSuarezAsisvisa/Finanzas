namespace FinanzasMCP.Application.Common.DTOs;

public sealed record PurchaseGoalContributionSummary(Guid Id, Guid PurchaseGoalId, Guid? TransactionId, decimal Amount, DateTimeOffset ContributionDate);
