using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.Goals.Queries;

public sealed record GetFinancialGoalsQuery(FinancialGoalType? Type = null);
