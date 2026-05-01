namespace FinanzasMCP.Application.SavingGoals.Commands;

public sealed record AddSavingGoalContributionCommand(Guid GoalId, decimal Amount, DateTimeOffset ContributionDate, Guid? TransactionId);
