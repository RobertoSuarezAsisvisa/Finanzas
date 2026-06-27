using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Domain.Tests;

public sealed class FinancialGoalTests
{
    [Fact]
    public void Financial_goal_calculates_suggested_monthly_contribution()
    {
        var goal = FinancialGoal.Create("Emergencias", 1200m, targetDate: new DateTimeOffset(2026, 12, 15, 0, 0, 0, TimeSpan.Zero));
        goal.AddContribution(300m);

        var suggested = goal.GetSuggestedMonthlyContribution(new DateTimeOffset(2026, 6, 15, 0, 0, 0, TimeSpan.Zero));

        Assert.Equal(150m, suggested);
    }

    [Fact]
    public void Financial_goal_rejects_non_positive_contribution()
    {
        var goal = FinancialGoal.Create("Emergencias", 1200m);

        Assert.Throws<InvalidOperationException>(() => goal.AddContribution(0m));
    }

    [Fact]
    public void Financial_goal_never_adjusts_current_amount_below_zero()
    {
        var goal = FinancialGoal.Create("Emergencias", 1200m);
        goal.AddContribution(100m);

        goal.AdjustContribution(-150m);

        Assert.Equal(0m, goal.CurrentAmount);
    }

    [Fact]
    public void Financial_goal_supports_status_transitions()
    {
        var goal = FinancialGoal.Create("Laptop", 1000m, FinancialGoalType.Purchase);

        goal.AddContribution(1000m);
        Assert.Equal(FinancialGoalStatus.Ready, goal.Status);

        goal.UpdateDetails(goal.Name, goal.TargetAmount, goal.Type, status: FinancialGoalStatus.Completed);
        Assert.Equal(FinancialGoalStatus.Completed, goal.Status);
        Assert.NotNull(goal.CompletedAt);

        goal.UpdateDetails(goal.Name, goal.TargetAmount, goal.Type, status: FinancialGoalStatus.Cancelled, completedAt: null);
        Assert.Equal(FinancialGoalStatus.Cancelled, goal.Status);
        Assert.Null(goal.CompletedAt);
    }
}
