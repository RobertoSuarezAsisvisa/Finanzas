namespace FinanzasMCP.Domain.Common;

public abstract class UserOwnedEntity : SoftDeletableEntity, IUserOwnedEntity
{
    public Guid UserId { get; private set; }

    public void AssignUser(Guid userId)
    {
        if (UserId != Guid.Empty && UserId != userId)
        {
            throw new InvalidOperationException("This record already belongs to another user.");
        }

        UserId = userId;
    }
}
