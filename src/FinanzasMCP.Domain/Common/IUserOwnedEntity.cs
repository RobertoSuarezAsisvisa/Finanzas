namespace FinanzasMCP.Domain.Common;

public interface IUserOwnedEntity
{
    Guid UserId { get; }
    void AssignUser(Guid userId);
}
