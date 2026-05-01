namespace FinanzasMCP.Domain.Common;

public abstract class SoftDeletableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DeletedAt { get; protected set; }

    public void MarkUpdated() => UpdatedAt = DateTimeOffset.UtcNow;
    public void SoftDelete() => DeletedAt = DateTimeOffset.UtcNow;
    public void Restore() => DeletedAt = null;
    public bool IsDeleted => DeletedAt is not null;
}
