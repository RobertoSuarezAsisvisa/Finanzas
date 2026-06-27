using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Application.Goals.Handlers;

internal static class FinancialGoalMapping
{
    public static FinancialGoalSummary ToSummary(FinancialGoal goal)
        => new(
            goal.Id,
            goal.Name,
            goal.Description,
            goal.TargetAmount,
            goal.CurrentAmount,
            goal.GetSuggestedMonthlyContribution(DateTimeOffset.UtcNow),
            goal.AccountId,
            goal.TargetDate,
            goal.Status,
            goal.Type,
            goal.Priority,
            goal.Url,
            goal.CompletedAt);

    public static FinancialGoalContributionSummary ToSummary(FinancialGoalContribution contribution)
        => new(
            contribution.Id,
            contribution.GoalId,
            contribution.TransactionId,
            contribution.Transaction?.AccountId,
            contribution.Amount,
            contribution.ContributionDate);
}
