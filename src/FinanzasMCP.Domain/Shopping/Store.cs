using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Shopping;

public sealed class Store : UserOwnedEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    public static Store Create(string name, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Store name is required.");
        }

        return new Store
        {
            Name = name.Trim(),
            Notes = notes?.Trim()
        };
    }

    public void UpdateDetails(string name, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Store name is required.");
        }

        Name = name.Trim();
        Notes = notes?.Trim();
        MarkUpdated();
    }
}
