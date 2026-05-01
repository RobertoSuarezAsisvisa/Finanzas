using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.SavingGoals.Commands;

public sealed record UpdateSavingGoalCommand(Guid Id, string Name, decimal TargetAmount, Guid? AccountId, DateTimeOffset? TargetDate, SavingGoalStatus? Status);
