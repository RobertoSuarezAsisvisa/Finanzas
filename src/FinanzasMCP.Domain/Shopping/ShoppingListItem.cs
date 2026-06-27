using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Shopping;

public sealed class ShoppingListItem : UserOwnedEntity
{
    public Guid ShoppingListId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public decimal DesiredQuantity { get; private set; }
    public ShoppingUnit Unit { get; private set; }
    public string? Notes { get; private set; }
    public ShoppingList ShoppingList { get; private set; } = null!;
    public ProductVariant ProductVariant { get; private set; } = null!;

    public static ShoppingListItem Create(Guid shoppingListId, Guid productVariantId, decimal desiredQuantity, ShoppingUnit unit, string? notes = null)
    {
        if (desiredQuantity <= 0)
        {
            throw new InvalidOperationException("Desired quantity must be positive.");
        }

        return new ShoppingListItem
        {
            ShoppingListId = shoppingListId,
            ProductVariantId = productVariantId,
            DesiredQuantity = desiredQuantity,
            Unit = unit,
            Notes = notes?.Trim()
        };
    }
}
