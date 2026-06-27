using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Shopping;

public sealed class ReceiptImportLine : UserOwnedEntity
{
    public Guid ReceiptImportId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string VariantName { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public ShoppingUnit Unit { get; private set; }
    public decimal TotalPrice { get; private set; }
    public decimal UnitPrice { get; private set; }
    public Guid? ProductVariantId { get; private set; }
    public ReceiptImport ReceiptImport { get; private set; } = null!;
    public ProductVariant? ProductVariant { get; private set; }

    public static ReceiptImportLine Create(Guid receiptImportId, string productName, string variantName, decimal quantity, ShoppingUnit unit, decimal totalPrice, Guid? productVariantId = null)
    {
        if (string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(variantName))
        {
            throw new InvalidOperationException("Receipt product and variant names are required.");
        }

        return new ReceiptImportLine
        {
            ReceiptImportId = receiptImportId,
            ProductName = productName.Trim(),
            VariantName = variantName.Trim(),
            Quantity = quantity,
            Unit = unit,
            TotalPrice = totalPrice,
            UnitPrice = StoreProductPrice.CalculateUnitPrice(totalPrice, quantity),
            ProductVariantId = productVariantId
        };
    }
}
