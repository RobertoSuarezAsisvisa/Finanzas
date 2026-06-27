using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Contributions;

namespace FinanzasMCP.Domain.Goals;

public sealed class FinancialGoal : UserOwnedEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal TargetAmount { get; private set; }
    public decimal CurrentAmount { get; private set; }
    public Guid? AccountId { get; private set; }
    public DateTimeOffset? TargetDate { get; private set; }
    public FinancialGoalStatus Status { get; private set; }
    public FinancialGoalType Type { get; private set; }
    public int Priority { get; private set; } = 1;
    public string? Url { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public Account? Account { get; private set; }
    public ICollection<FinancialGoalContribution> Contributions { get; private set; } = new List<FinancialGoalContribution>();

    public decimal? GetSuggestedMonthlyContribution(DateTimeOffset referenceDate)
    {
        if (TargetDate is null)
        {
            return null;
        }

        var remainingAmount = TargetAmount - CurrentAmount;
        if (remainingAmount <= 0m)
        {
            return 0m;
        }

        var monthsRemaining = ((TargetDate.Value.Year - referenceDate.Year) * 12)
            + TargetDate.Value.Month - referenceDate.Month;

        if (TargetDate.Value.Day > referenceDate.Day)
        {
            monthsRemaining += 1;
        }

        if (monthsRemaining <= 0)
        {
            return remainingAmount;
        }

        return decimal.Round(remainingAmount / monthsRemaining, 2, MidpointRounding.AwayFromZero);
    }

    public static FinancialGoal Create(
        string name,
        decimal targetAmount,
        FinancialGoalType type = FinancialGoalType.Saving,
        string? description = null,
        int priority = 1,
        string? url = null,
        Guid? accountId = null,
        DateTimeOffset? targetDate = null)
    {
        Validate(name, targetAmount, priority);
        return new FinancialGoal
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            TargetAmount = targetAmount,
            CurrentAmount = 0m,
            AccountId = accountId,
            TargetDate = targetDate,
            Status = FinancialGoalStatus.InProgress,
            Type = type,
            Priority = priority,
            Url = url?.Trim()
        };
    }

    public void AddContribution(decimal amount)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Contribution amount must be positive.");
        }

        CurrentAmount += amount;
        RefreshProgressStatus();
        MarkUpdated();
    }

    public void AdjustContribution(decimal delta)
    {
        CurrentAmount += delta;
        if (CurrentAmount < 0m)
        {
            CurrentAmount = 0m;
        }

        RefreshProgressStatus();
        MarkUpdated();
    }

    public void UpdateDetails(
        string name,
        decimal targetAmount,
        FinancialGoalType type,
        string? description = null,
        int priority = 1,
        string? url = null,
        Guid? accountId = null,
        DateTimeOffset? targetDate = null,
        FinancialGoalStatus? status = null,
        DateTimeOffset? completedAt = null)
    {
        Validate(name, targetAmount, priority);
        Name = name.Trim();
        Description = description?.Trim();
        TargetAmount = targetAmount;
        Type = type;
        Priority = priority;
        Url = url?.Trim();
        AccountId = accountId;
        TargetDate = targetDate;

        if (status is not null)
        {
            Status = status.Value;
        }
        else
        {
            RefreshProgressStatus();
        }

        CompletedAt = completedAt;
        if (Status == FinancialGoalStatus.Completed && CompletedAt is null)
        {
            CompletedAt = DateTimeOffset.UtcNow;
        }

        if (Status != FinancialGoalStatus.Completed)
        {
            CompletedAt = completedAt;
        }

        MarkUpdated();
    }

    private void RefreshProgressStatus()
    {
        if (Status is FinancialGoalStatus.Completed or FinancialGoalStatus.Cancelled)
        {
            return;
        }

        Status = CurrentAmount >= TargetAmount ? FinancialGoalStatus.Ready : FinancialGoalStatus.InProgress;
    }

    private static void Validate(string name, decimal targetAmount, int priority)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Goal name is required.");
        }

        if (targetAmount <= 0)
        {
            throw new InvalidOperationException("Target amount must be positive.");
        }

        if (priority <= 0)
        {
            throw new InvalidOperationException("Priority must be positive.");
        }
    }
}
