using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Categories;
using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Recurring;
using FinanzasMCP.Domain.Tags;

namespace FinanzasMCP.Domain.Transactions;

public sealed class Transaction : SoftDeletableEntity
{
    public TransactionType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public Guid AccountId { get; private set; }
    public Guid? ToAccountId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string? Description { get; private set; }
    public string? Reference { get; private set; }
    public DateTimeOffset TransactionDate { get; private set; }
    public Guid? RecurringRuleId { get; private set; }
    public Account Account { get; private set; } = null!;
    public Account? ToAccount { get; private set; }
    public Category? Category { get; private set; }
    public RecurringRule? RecurringRule { get; private set; }
    public ICollection<TransactionTag> Tags { get; private set; } = new List<TransactionTag>();

    public ICollection<SavingGoalContribution> SavingGoalContributions { get; private set; } = new List<SavingGoalContribution>();

    public static Transaction Create(
        TransactionType type,
        decimal amount,
        string currency,
        Guid accountId,
        Guid? toAccountId,
        Guid? categoryId,
        string? description,
        string? reference,
        DateTimeOffset transactionDate,
        Guid? recurringRuleId)
        => new()
        {
            Type = type,
            Amount = amount,
            Currency = currency.Trim().ToUpperInvariant(),
            AccountId = accountId,
            ToAccountId = toAccountId,
            CategoryId = categoryId,
            Description = description?.Trim(),
            Reference = reference?.Trim(),
            TransactionDate = transactionDate,
            RecurringRuleId = recurringRuleId
        };

    public void UpdateDetails(
        TransactionType type,
        decimal amount,
        string currency,
        Guid accountId,
        Guid? toAccountId,
        Guid? categoryId,
        string? description,
        string? reference,
        DateTimeOffset transactionDate,
        Guid? recurringRuleId)
    {
        Type = type;
        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
        AccountId = accountId;
        ToAccountId = toAccountId;
        CategoryId = categoryId;
        Description = description?.Trim();
        Reference = reference?.Trim();
        TransactionDate = transactionDate;
        RecurringRuleId = recurringRuleId;
        MarkUpdated();
    }
}
