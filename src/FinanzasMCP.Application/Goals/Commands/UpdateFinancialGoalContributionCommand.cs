namespace FinanzasMCP.Application.Goals.Commands;

public sealed record UpdateFinancialGoalContributionCommand(Guid Id, decimal Amount, DateTimeOffset ContributionDate, Guid? TransactionId, Guid? AccountId);
