using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Categories;
using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Recurring;

public sealed class RecurringRule : SoftDeletableEntity
{
    public string Name { get; private set; } = string.Empty;
    public RecurringType Type { get; private set; }
    public decimal Amount { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public RecurringFrequency Frequency { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset? EndDate { get; private set; }
    public DateTimeOffset NextDueDate { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Account Account { get; private set; } = null!;
    public Category? Category { get; private set; }

    public static RecurringRule Create(
        string name,
        RecurringType type,
        decimal amount,
        Guid accountId,
        Guid? categoryId,
        RecurringFrequency frequency,
        DateTimeOffset startDate,
        DateTimeOffset? endDate,
        DateTimeOffset nextDueDate)
        => new()
        {
            Name = name.Trim(),
            Type = type,
            Amount = amount,
            AccountId = accountId,
            CategoryId = categoryId,
            Frequency = frequency,
            StartDate = startDate,
            EndDate = endDate,
            NextDueDate = nextDueDate,
            IsActive = true
        };

    public void UpdateDetails(
        string name,
        RecurringType type,
        decimal amount,
        Guid accountId,
        Guid? categoryId,
        RecurringFrequency frequency,
        DateTimeOffset startDate,
        DateTimeOffset? endDate,
        DateTimeOffset nextDueDate,
        bool isActive)
    {
        Name = name.Trim();
        Type = type;
        Amount = amount;
        AccountId = accountId;
        CategoryId = categoryId;
        Frequency = frequency;
        StartDate = startDate;
        EndDate = endDate;
        NextDueDate = nextDueDate;
        IsActive = isActive;
        MarkUpdated();
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
