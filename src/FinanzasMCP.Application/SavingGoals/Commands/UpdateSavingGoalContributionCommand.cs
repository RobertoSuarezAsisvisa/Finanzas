namespace FinanzasMCP.Application.SavingGoals.Commands;

public sealed record UpdateSavingGoalContributionCommand(Guid Id, decimal Amount, DateTimeOffset ContributionDate, Guid? TransactionId);
