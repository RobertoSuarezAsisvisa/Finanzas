using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Contributions;

namespace FinanzasMCP.Domain.Goals;

public sealed class SavingGoal : SoftDeletableEntity
{
    public string Name { get; private set; } = string.Empty;
    public decimal GoalAmount { get; private set; } // Monto objetivo para alcanzar la meta de ahorro
    public decimal CurrentAmount { get; private set; }
    public Guid? AccountId { get; private set; }
    public DateTimeOffset? GoalDate { get; private set; } // Fecha objetivo para alcanzar la meta de ahorro
    public SavingGoalStatus Status { get; private set; }
    public Account? Account { get; private set; }

    public ICollection<SavingGoalContribution> Contributions { get; private set; } = new List<SavingGoalContribution>();

    public decimal? GetSuggestedMonthlyContribution(DateTimeOffset referenceDate)
    {
        if (GoalDate is null)
        {
            return null;
        }

        var remainingAmount = GoalAmount - CurrentAmount;
        if (remainingAmount <= 0m)
        {
            return 0m;
        }

        var monthsRemaining = ((GoalDate.Value.Year - referenceDate.Year) * 12)
            + GoalDate.Value.Month - referenceDate.Month;

        if (GoalDate.Value.Day > referenceDate.Day)
        {
            monthsRemaining += 1;
        }

        if (monthsRemaining <= 0)
        {
            return remainingAmount;
        }

        return decimal.Round(remainingAmount / monthsRemaining, 2, MidpointRounding.AwayFromZero);
    }

    public static SavingGoal Create(string name, decimal targetAmount, Guid? accountId = null, DateTimeOffset? targetDate = null)
        => new()
        {
            Name = name.Trim(),
            GoalAmount = targetAmount,
            AccountId = accountId,
            GoalDate = targetDate,
            CurrentAmount = 0m,
            Status = SavingGoalStatus.InProgress
        };

    public void AddContribution(decimal amount)
    {
        CurrentAmount += amount;
        MarkUpdated();
    }

    public void UpdateDetails(string name, decimal targetAmount, Guid? accountId = null, DateTimeOffset? targetDate = null, SavingGoalStatus? status = null)
    {
        Name = name.Trim();
        GoalAmount = targetAmount;
        AccountId = accountId;
        GoalDate = targetDate;
        if (status is not null)
        {
            Status = status.Value;
        }
        MarkUpdated();
    }
}
