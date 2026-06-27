using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Shopping;

public sealed class ProductVariant : UserOwnedEntity
{
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal NormalizedQuantity { get; private set; }
    public ShoppingUnit Unit { get; private set; }
    public string? Barcode { get; private set; }
    public Product Product { get; private set; } = null!;
    public ICollection<StoreProductPrice> Prices { get; private set; } = new List<StoreProductPrice>();

    public static ProductVariant Create(Guid productId, string name, decimal normalizedQuantity, ShoppingUnit unit, string? barcode = null)
    {
        Validate(name, normalizedQuantity);
        return new ProductVariant
        {
            ProductId = productId,
            Name = name.Trim(),
            NormalizedQuantity = normalizedQuantity,
            Unit = unit,
            Barcode = barcode?.Trim()
        };
    }

    public void UpdateDetails(string name, decimal normalizedQuantity, ShoppingUnit unit, string? barcode = null)
    {
        Validate(name, normalizedQuantity);
        Name = name.Trim();
        NormalizedQuantity = normalizedQuantity;
        Unit = unit;
        Barcode = barcode?.Trim();
        MarkUpdated();
    }

    private static void Validate(string name, decimal normalizedQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Variant name is required.");
        }

        if (normalizedQuantity <= 0)
        {
            throw new InvalidOperationException("Normalized quantity must be positive.");
        }
    }
}
