using FinanzasMCP.Domain.Tags;
using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Transactions;

public sealed class TransactionTag : IUserOwnedEntity
{
    public Guid UserId { get; private set; }
    public Guid TransactionId { get; private set; }
    public Guid TagId { get; private set; }

    public Tag? Tag { get; private set; }

    public void AssignUser(Guid userId)
    {
        if (UserId != Guid.Empty && UserId != userId)
        {
            throw new InvalidOperationException("This record already belongs to another user.");
        }

        UserId = userId;
    }

    public static TransactionTag Create(Guid transactionId, Guid tagId)
        => new()
        {
            TransactionId = transactionId,
            TagId = tagId
        };
}
