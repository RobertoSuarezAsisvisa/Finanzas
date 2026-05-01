namespace FinanzasMCP.Application.SavingGoals.Commands;

public sealed record CreateSavingGoalCommand(string Name, decimal TargetAmount, Guid? AccountId, DateTimeOffset? TargetDate);
