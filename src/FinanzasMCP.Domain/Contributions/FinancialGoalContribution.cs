using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Goals;
using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Domain.Contributions;

public sealed class FinancialGoalContribution : UserOwnedEntity
{
    public Guid GoalId { get; private set; }
    public Guid? TransactionId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset ContributionDate { get; private set; }
    public Transaction? Transaction { get; private set; }
    public FinancialGoal Goal { get; private set; } = null!;

    public static FinancialGoalContribution Create(Guid goalId, decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null)
    {
        Validate(amount);
        return new FinancialGoalContribution
        {
            GoalId = goalId,
            Amount = amount,
            ContributionDate = contributionDate,
            TransactionId = transactionId
        };
    }

    public void UpdateDetails(decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null)
    {
        Validate(amount);
        Amount = amount;
        ContributionDate = contributionDate;
        TransactionId = transactionId;
        MarkUpdated();
    }

    private static void Validate(decimal amount)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Contribution amount must be positive.");
        }
    }
}
