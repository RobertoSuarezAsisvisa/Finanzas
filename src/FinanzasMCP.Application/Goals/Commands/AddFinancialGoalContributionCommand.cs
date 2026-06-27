namespace FinanzasMCP.Application.Goals.Commands;

public sealed record AddFinancialGoalContributionCommand(Guid GoalId, decimal Amount, DateTimeOffset ContributionDate, Guid? TransactionId, Guid? AccountId);
