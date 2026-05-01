namespace FinanzasMCP.Application.PurchaseGoals.Commands;

public sealed record AddPurchaseGoalContributionCommand(Guid PurchaseGoalId, decimal Amount, DateTimeOffset ContributionDate, Guid? TransactionId);
