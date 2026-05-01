using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.Domain.Contributions;

public sealed class PurchaseGoalContribution : SoftDeletableEntity
{
    public Guid PurchaseGoalId { get; private set; }
    public Guid? TransactionId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTimeOffset ContributionDate { get; private set; }

    public PurchaseGoal PurchaseGoal { get; private set; } = null!;

    public static PurchaseGoalContribution Create(Guid purchaseGoalId, decimal amount, DateTimeOffset contributionDate, Guid? transactionId = null)
        => new()
        {
            PurchaseGoalId = purchaseGoalId,
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
