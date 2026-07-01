using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Budgets;

public sealed class Budget : UserOwnedEntity
{
    public string Name { get; private set; } = string.Empty;
    public decimal LimitAmount { get; private set; }
    public PeriodType PeriodType { get; private set; }
    public BudgetValidityType ValidityType { get; private set; }
    public DateTimeOffset? PeriodStart { get; private set; }
    public DateTimeOffset? PeriodEnd { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static Budget Create(string name, decimal limitAmount, PeriodType periodType, BudgetValidityType validityType, DateTimeOffset? periodStart = null, DateTimeOffset? periodEnd = null)
    {
        ValidateDetails(name, limitAmount, periodType);
        ValidatePeriod(validityType, periodStart, periodEnd);
        return new Budget
        {
            Name = name.Trim(),
            LimitAmount = limitAmount,
            PeriodType = periodType,
            ValidityType = validityType,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            IsActive = true
        };
    }

    public void UpdateDetails(string name, decimal limitAmount, PeriodType periodType, BudgetValidityType validityType, DateTimeOffset? periodStart, DateTimeOffset? periodEnd, bool isActive)
    {
        ValidateDetails(name, limitAmount, periodType);
        ValidatePeriod(validityType, periodStart, periodEnd);

        Name = name.Trim();
        LimitAmount = limitAmount;
        PeriodType = periodType;
        ValidityType = validityType;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        IsActive = isActive;
        MarkUpdated();
    }

    private static void ValidateDetails(string name, decimal limitAmount, PeriodType periodType)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Budget name is required.");
        }

        if (limitAmount <= 0)
        {
            throw new InvalidOperationException("Budget limit must be positive.");
        }

        if (!Enum.IsDefined(periodType))
        {
            throw new InvalidOperationException("Budget period type is invalid.");
        }
    }

    private static void ValidatePeriod(BudgetValidityType validityType, DateTimeOffset? periodStart, DateTimeOffset? periodEnd)
    {
        if (validityType == BudgetValidityType.FixedPeriod)
        {
            if (periodStart is null || periodEnd is null)
            {
                throw new InvalidOperationException("Fixed period budgets require start and end dates.");
            }

            if (periodEnd <= periodStart)
            {
                throw new InvalidOperationException("Period end must be later than period start.");
            }
        }
        else
        {
            if (periodEnd is not null && periodStart is null)
            {
                throw new InvalidOperationException("An indefinite budget cannot define only an end date.");
            }
        }
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkUpdated();
    }
}
