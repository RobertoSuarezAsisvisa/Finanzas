using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Domain.Tags;

public sealed class Tag : SoftDeletableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Color { get; private set; }

    public ICollection<TransactionTag> TransactionTags { get; private set; } = new List<TransactionTag>();

    public static Tag Create(string name, string? color = null)
        => new()
        {
            Name = name.Trim(),
            Color = color?.Trim()
        };

    public void UpdateDetails(string name, string? color = null)
    {
        Name = name.Trim();
        Color = color?.Trim();
        MarkUpdated();
    }
}
