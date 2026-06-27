using FinanzasMCP.Domain.Shopping;

namespace FinanzasMCP.Application.Shopping;

public sealed record StoreSummary(Guid Id, string Name, string? Notes);
public sealed record ProductSummary(Guid Id, string Name, Guid? CategoryId, string? Notes);
public sealed record ProductVariantSummary(Guid Id, Guid ProductId, string ProductName, string Name, decimal NormalizedQuantity, ShoppingUnit Unit, string? Barcode);
public sealed record StoreProductPriceSummary(Guid Id, Guid StoreId, string StoreName, Guid ProductVariantId, string ProductName, string VariantName, decimal TotalPrice, decimal NormalizedQuantity, ShoppingUnit Unit, decimal UnitPrice, DateTimeOffset ObservedAt, StoreProductPriceSource Source, Guid? ReceiptImportId, string? Notes);
public sealed record ShoppingListSummary(Guid Id, string Name, DateTimeOffset ListDate, Guid? TransactionId, IReadOnlyList<ShoppingListItemSummary> Items);
public sealed record ShoppingListItemSummary(Guid Id, Guid ProductVariantId, string ProductName, string VariantName, decimal DesiredQuantity, ShoppingUnit Unit, string? Notes);
public sealed record ReceiptImportSummary(Guid Id, Guid? StoreId, string? StoreName, string? DetectedStoreName, DateTimeOffset? ReceiptDate, string FileName, string ContentType, long SizeBytes, ReceiptImportStatus Status, IReadOnlyList<ReceiptImportLineSummary> Lines);
public sealed record ReceiptImportLineSummary(Guid Id, string ProductName, string VariantName, decimal Quantity, ShoppingUnit Unit, decimal TotalPrice, decimal UnitPrice, Guid? ProductVariantId);

public sealed record CreateStoreCommand(string Name, string? Notes);
public sealed record UpdateStoreCommand(Guid Id, string Name, string? Notes);
public sealed record CreateProductCommand(string Name, Guid? CategoryId, string? Notes);
public sealed record UpdateProductCommand(Guid Id, string Name, Guid? CategoryId, string? Notes);
public sealed record CreateProductVariantCommand(Guid ProductId, string Name, decimal NormalizedQuantity, ShoppingUnit Unit, string? Barcode);
public sealed record UpdateProductVariantCommand(Guid Id, string Name, decimal NormalizedQuantity, ShoppingUnit Unit, string? Barcode);
public sealed record CreateStoreProductPriceCommand(Guid StoreId, Guid ProductVariantId, decimal TotalPrice, decimal NormalizedQuantity, ShoppingUnit Unit, DateTimeOffset ObservedAt, string? Notes);
public sealed record UpdateStoreProductPriceCommand(Guid Id, decimal TotalPrice, decimal NormalizedQuantity, ShoppingUnit Unit, DateTimeOffset ObservedAt, string? Notes);
public sealed record CreateShoppingListCommand(string Name, DateTimeOffset ListDate, Guid? TransactionId, IReadOnlyList<CreateShoppingListItemCommand> Items);
public sealed record CreateShoppingListItemCommand(Guid ProductVariantId, decimal DesiredQuantity, ShoppingUnit Unit, string? Notes);
public sealed record ParsedReceiptLine(string ProductName, string VariantName, decimal Quantity, ShoppingUnit Unit, decimal TotalPrice);
public sealed record ParsedReceipt(string? StoreName, DateTimeOffset? ReceiptDate, IReadOnlyList<ParsedReceiptLine> Lines);
public sealed record ConfirmReceiptImportCommand(Guid StoreId, IReadOnlyList<ConfirmReceiptImportLineCommand> Lines);
public sealed record ConfirmReceiptImportLineCommand(string ProductName, string VariantName, decimal Quantity, ShoppingUnit Unit, decimal TotalPrice, Guid? ProductVariantId);
public sealed record ShoppingRecommendationSummary(Guid ShoppingListId, decimal EstimatedTotal, decimal EstimatedSavings, IReadOnlyList<ShoppingRecommendationStoreGroup> StoreGroups, IReadOnlyList<ShoppingRecommendationMissingItem> MissingItems);
public sealed record ShoppingRecommendationStoreGroup(Guid StoreId, string StoreName, decimal Subtotal, IReadOnlyList<ShoppingRecommendationItem> Items);
public sealed record ShoppingRecommendationItem(Guid ProductVariantId, string ProductName, string VariantName, decimal DesiredQuantity, ShoppingUnit Unit, decimal UnitPrice, decimal EstimatedPrice, DateTimeOffset ObservedAt);
public sealed record ShoppingRecommendationMissingItem(Guid ProductVariantId, string ProductName, string VariantName, decimal DesiredQuantity, ShoppingUnit Unit);

public interface IReceiptParser
{
    Task<ParsedReceipt> ParseAsync(Stream image, string contentType, CancellationToken cancellationToken);
}
