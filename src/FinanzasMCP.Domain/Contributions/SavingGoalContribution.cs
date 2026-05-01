using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Goals;
using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Domain.Contributions;

public sealed class SavingGoalContribution : SoftDeletableEntity
{
    public Guid GoalId { get; private set; }
    public Guid? TransactionId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset ContributionDate { get; private set; }

    public Transaction? Transaction { get; private set; }

    public SavingGoal SavingGoal { get; private set; } = null!;

    public static SavingGoalContribution Create(Guid goalId, decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null)
        => new()
        {
            GoalId = goalId,
            Amount = amount,
            ContributionDate = contributionDate,
            TransactionId = transactionId
        };

    public void UpdateDetails(decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null)
    {
        Amount = amount;
        ContributionDate = contributionDate;
        TransactionId = transactionId;
        MarkUpdated();
    }
}
