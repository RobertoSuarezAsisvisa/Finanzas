using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Contributions;

namespace FinanzasMCP.Domain.Goals;

public sealed class PurchaseGoal : SoftDeletableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal GoalPrice { get; private set; } // Precio objetivo para alcanzar la meta de compra
    public decimal SavedAmount { get; private set; }
    public int Priority { get; private set; } = 1;
    public string? Url { get; private set; }
    public Guid? AccountId { get; private set; }
    public DateTimeOffset? TargetDate { get; private set; } // Fecha objetivo para alcanzar la meta de compra
    public PurchaseGoalStatus Status { get; private set; }
    public DateTimeOffset? PurchasedAt { get; private set; }
    public Account? Account { get; private set; }

    public ICollection<PurchaseGoalContribution> Contributions { get; private set; } = new List<PurchaseGoalContribution>();

    public decimal? GetSuggestedMonthlyContribution(DateTimeOffset referenceDate)
    {
        if (TargetDate is null)
        {
            return null;
        }

        var remainingAmount = GoalPrice - SavedAmount;
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

    public static PurchaseGoal Create(string name, decimal goalPrice, string? description = null, int priority = 1, string? url = null, Guid? accountId = null, DateTimeOffset? targetDate = null)
        => new()
        {
            Name = name.Trim(),
            GoalPrice = goalPrice,
            Description = description?.Trim(),
            Priority = priority,
            Url = url?.Trim(),
            AccountId = accountId,
            TargetDate = targetDate,
            SavedAmount = 0m,
            Status = PurchaseGoalStatus.Saving
        };

    public void AddContribution(decimal amount)
    {
        SavedAmount += amount;
        MarkUpdated();
    }

    public void UpdateDetails(string name, decimal goalPrice, string? description = null, int priority = 1, string? url = null, Guid? accountId = null, DateTimeOffset? targetDate = null, PurchaseGoalStatus? status = null, DateTimeOffset? purchasedAt = null)
    {
        Name = name.Trim();
        GoalPrice = goalPrice;
        Description = description?.Trim();
        Priority = priority;
        Url = url?.Trim();
        AccountId = accountId;
        TargetDate = targetDate;
        if (status is not null)
        {
            Status = status.Value;
        }
        PurchasedAt = purchasedAt;
        MarkUpdated();
    }
}
