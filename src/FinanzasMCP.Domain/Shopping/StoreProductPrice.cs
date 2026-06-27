using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Shopping;

public sealed class StoreProductPrice : UserOwnedEntity
{
    public Guid StoreId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal NormalizedQuantity { get; private set; }
    public ShoppingUnit Unit { get; private set; }
    public decimal UnitPrice { get; private set; }
    public DateTimeOffset ObservedAt { get; private set; }
    public StoreProductPriceSource Source { get; private set; }
    public Guid? ReceiptImportId { get; private set; }
    public string? Notes { get; private set; }
    public Store Store { get; private set; } = null!;
    public ProductVariant ProductVariant { get; private set; } = null!;
    public ReceiptImport? ReceiptImport { get; private set; }

    public static StoreProductPrice Create(
        Guid storeId,
        Guid productVariantId,
        decimal totalPrice,
        decimal normalizedQuantity,
        ShoppingUnit unit,
        DateTimeOffset observedAt,
        StoreProductPriceSource source,
        Guid? receiptImportId = null,
        string? notes = null)
    {
        Validate(totalPrice, normalizedQuantity);
        return new StoreProductPrice
        {
            StoreId = storeId,
            ProductVariantId = productVariantId,
            TotalPrice = totalPrice,
            NormalizedQuantity = normalizedQuantity,
            Unit = unit,
            UnitPrice = CalculateUnitPrice(totalPrice, normalizedQuantity),
            ObservedAt = observedAt,
            Source = source,
            ReceiptImportId = receiptImportId,
            Notes = notes?.Trim()
        };
    }

    public void UpdateDetails(decimal totalPrice, decimal normalizedQuantity, ShoppingUnit unit, DateTimeOffset observedAt, string? notes = null)
    {
        Validate(totalPrice, normalizedQuantity);
        TotalPrice = totalPrice;
        NormalizedQuantity = normalizedQuantity;
        Unit = unit;
        UnitPrice = CalculateUnitPrice(totalPrice, normalizedQuantity);
        ObservedAt = observedAt;
        Notes = notes?.Trim();
        MarkUpdated();
    }

    public static decimal CalculateUnitPrice(decimal totalPrice, decimal normalizedQuantity)
    {
        Validate(totalPrice, normalizedQuantity);
        return Math.Round(totalPrice / normalizedQuantity, 6, MidpointRounding.AwayFromZero);
    }

    private static void Validate(decimal totalPrice, decimal normalizedQuantity)
    {
        if (totalPrice <= 0)
        {
            throw new InvalidOperationException("Total price must be positive.");
        }

        if (normalizedQuantity <= 0)
        {
            throw new InvalidOperationException("Normalized quantity must be positive.");
        }
    }
}
