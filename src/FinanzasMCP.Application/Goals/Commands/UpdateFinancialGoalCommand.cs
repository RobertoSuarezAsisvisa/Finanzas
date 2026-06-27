using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.Goals.Commands;

public sealed record UpdateFinancialGoalCommand(
    Guid Id,
    string Name,
    decimal TargetAmount,
    FinancialGoalType Type,
    string? Description = null,
    int Priority = 1,
    string? Url = null,
    Guid? AccountId = null,
    DateTimeOffset? TargetDate = null,
    FinancialGoalStatus? Status = null,
    DateTimeOffset? CompletedAt = null);
