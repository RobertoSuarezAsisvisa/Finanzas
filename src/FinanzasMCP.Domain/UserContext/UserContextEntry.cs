using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.UserContext;

public sealed class UserContextEntry : SoftDeletableEntity
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public new DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public static UserContextEntry Create(string key, string value)
        => new()
        {
            Key = key.Trim(),
            Value = value
        };

    public void UpdateValue(string value)
    {
        Value = value;
        UpdatedAt = DateTimeOffset.UtcNow;
        MarkUpdated();
    }
}
