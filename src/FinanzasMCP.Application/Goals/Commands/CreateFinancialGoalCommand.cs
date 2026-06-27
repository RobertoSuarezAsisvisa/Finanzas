using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.Goals.Commands;

public sealed record CreateFinancialGoalCommand(
    string Name,
    decimal TargetAmount,
    FinancialGoalType Type = FinancialGoalType.Saving,
    string? Description = null,
    int Priority = 1,
    string? Url = null,
    Guid? AccountId = null,
    DateTimeOffset? TargetDate = null);
