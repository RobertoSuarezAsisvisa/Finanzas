namespace FinanzasMCP.Application.PurchaseGoals.Commands;

public sealed record UpdatePurchaseGoalContributionCommand(Guid Id, decimal Amount, DateTimeOffset ContributionDate, Guid? TransactionId);
