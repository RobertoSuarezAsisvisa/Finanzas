using FinanzasMCP.Domain.Common;
using FinanzasMCP.Domain.Transactions;

namespace FinanzasMCP.Domain.Tags;

public sealed class Tag : UserOwnedEntity
{
    public const int MaxNameLength = 120;
    public const int MaxColorLength = 20;

    public string Name { get; private set; } = string.Empty;
    public string? Color { get; private set; }

    public ICollection<TransactionTag> TransactionTags { get; private set; } = new List<TransactionTag>();

    public static Tag Create(string name, string? color = null)
        => new()
        {
            Name = NormalizeName(name),
            Color = NormalizeColor(color)
        };

    public void UpdateDetails(string name, string? color = null)
    {
        Name = NormalizeName(name);
        Color = NormalizeColor(color);
        MarkUpdated();
    }

    private static string NormalizeName(string name)
    {
        var normalized = (name ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new InvalidOperationException("Tag name is required.");
        }

        if (normalized.Length > MaxNameLength)
        {
            throw new InvalidOperationException($"Tag name cannot exceed {MaxNameLength} characters.");
        }

        return normalized;
    }

    private static string? NormalizeColor(string? color)
    {
        var normalized = color?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return null;
        }

        if (normalized.Length > MaxColorLength)
        {
            throw new InvalidOperationException($"Tag color cannot exceed {MaxColorLength} characters.");
        }

        return normalized;
    }
}
