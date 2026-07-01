using FinanzasMCP.Domain.Budgets;

namespace FinanzasMCP.Domain.Tests;

public sealed class BudgetTests
{
    [Theory]
    [InlineData(PeriodType.Daily)]
    [InlineData(PeriodType.Weekly)]
    [InlineData(PeriodType.Monthly)]
    [InlineData(PeriodType.Yearly)]
    public void Can_create_budget_for_supported_periods(PeriodType periodType)
    {
        var budget = Budget.Create("Supermercado", 100m, periodType, BudgetValidityType.Indefinite);

        Assert.Equal(periodType, budget.PeriodType);
        Assert.True(budget.IsActive);
    }

    [Fact]
    public void Rejects_non_positive_limit()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            Budget.Create("Transporte", 0m, PeriodType.Monthly, BudgetValidityType.Indefinite));

        Assert.Equal("Budget limit must be positive.", exception.Message);
    }

    [Fact]
    public void Rejects_fixed_period_without_dates()
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            Budget.Create("Viaje", 100m, PeriodType.Monthly, BudgetValidityType.FixedPeriod));

        Assert.Equal("Fixed period budgets require start and end dates.", exception.Message);
    }

    [Fact]
    public void Rejects_fixed_period_with_invalid_range()
    {
        var start = DateTimeOffset.UtcNow;

        var exception = Assert.Throws<InvalidOperationException>(() =>
            Budget.Create("Viaje", 100m, PeriodType.Monthly, BudgetValidityType.FixedPeriod, start, start));

        Assert.Equal("Period end must be later than period start.", exception.Message);
    }

    [Fact]
    public void Update_changes_budget_details_without_category()
    {
        var budget = Budget.Create("Casa", 150m, PeriodType.Monthly, BudgetValidityType.Indefinite);

        budget.UpdateDetails("Casa", 175m, PeriodType.Weekly, BudgetValidityType.Indefinite, null, null, true);

        Assert.Equal(175m, budget.LimitAmount);
        Assert.Equal(PeriodType.Weekly, budget.PeriodType);
    }
}
