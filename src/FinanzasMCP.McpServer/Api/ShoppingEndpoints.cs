using FinanzasMCP.Application.Auth;
using FinanzasMCP.Application.Shopping;
using FinanzasMCP.Domain.Shopping;
using FinanzasMCP.McpServer.Storage;

namespace FinanzasMCP.McpServer.Api;

public static class ShoppingEndpoints
{
    public static void MapShoppingEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/shopping");

        group.MapGet("stores", async (ShoppingService service, CancellationToken ct) => Results.Ok(await service.ListStores(ct)));
        group.MapPost("stores", async (CreateStoreRequest request, ShoppingService service, CancellationToken ct) =>
            Results.Created("/api/v1/shopping/stores", await service.CreateStore(new CreateStoreCommand(request.Name, request.Notes), ct)));
        group.MapPut("stores/{id:guid}", async (Guid id, CreateStoreRequest request, ShoppingService service, CancellationToken ct) =>
            Results.Ok(await service.UpdateStore(new UpdateStoreCommand(id, request.Name, request.Notes), ct)));
        group.MapDelete("stores/{id:guid}", async (Guid id, ShoppingService service, CancellationToken ct) =>
        {
            await service.DeleteStore(id, ct);
            return Results.NoContent();
        });

        group.MapGet("products", async (ShoppingService service, CancellationToken ct) => Results.Ok(await service.ListProducts(ct)));
        group.MapPost("products", async (CreateProductRequest request, ShoppingService service, CancellationToken ct) =>
            Results.Created("/api/v1/shopping/products", await service.CreateProduct(new CreateProductCommand(request.Name, request.CategoryId, request.Notes), ct)));
        group.MapPut("products/{id:guid}", async (Guid id, CreateProductRequest request, ShoppingService service, CancellationToken ct) =>
            Results.Ok(await service.UpdateProduct(new UpdateProductCommand(id, request.Name, request.CategoryId, request.Notes), ct)));
        group.MapDelete("products/{id:guid}", async (Guid id, ShoppingService service, CancellationToken ct) =>
        {
            await service.DeleteProduct(id, ct);
            return Results.NoContent();
        });

        group.MapGet("variants", async (Guid? productId, ShoppingService service, CancellationToken ct) => Results.Ok(await service.ListVariants(productId, ct)));
        group.MapPost("variants", async (CreateProductVariantRequest request, ShoppingService service, CancellationToken ct) =>
            Results.Created("/api/v1/shopping/variants", await service.CreateVariant(new CreateProductVariantCommand(request.ProductId, request.Name, request.NormalizedQuantity, request.Unit, request.Barcode), ct)));
        group.MapPut("variants/{id:guid}", async (Guid id, CreateProductVariantRequest request, ShoppingService service, CancellationToken ct) =>
            Results.Ok(await service.UpdateVariant(new UpdateProductVariantCommand(id, request.Name, request.NormalizedQuantity, request.Unit, request.Barcode), ct)));
        group.MapDelete("variants/{id:guid}", async (Guid id, ShoppingService service, CancellationToken ct) =>
        {
            await service.DeleteVariant(id, ct);
            return Results.NoContent();
        });

        group.MapGet("prices", async (Guid? storeId, Guid? productVariantId, ShoppingService service, CancellationToken ct) => Results.Ok(await service.ListPrices(storeId, productVariantId, ct)));
        group.MapPost("prices", async (CreateStoreProductPriceRequest request, ShoppingService service, CancellationToken ct) =>
            Results.Created("/api/v1/shopping/prices", await service.CreatePrice(new CreateStoreProductPriceCommand(request.StoreId, request.ProductVariantId, request.TotalPrice, request.NormalizedQuantity, request.Unit, request.ObservedAt, request.Notes), ct)));
        group.MapPut("prices/{id:guid}", async (Guid id, CreateStoreProductPriceRequest request, ShoppingService service, CancellationToken ct) =>
            Results.Ok(await service.UpdatePrice(new UpdateStoreProductPriceCommand(id, request.TotalPrice, request.NormalizedQuantity, request.Unit, request.ObservedAt, request.Notes), ct)));
        group.MapDelete("prices/{id:guid}", async (Guid id, ShoppingService service, CancellationToken ct) =>
        {
            await service.DeletePrice(id, ct);
            return Results.NoContent();
        });

        group.MapGet("lists", async (ShoppingService service, CancellationToken ct) => Results.Ok(await service.ListShoppingLists(ct)));
        group.MapPost("lists", async (CreateShoppingListRequest request, ShoppingService service, CancellationToken ct) =>
            Results.Created("/api/v1/shopping/lists", await service.CreateShoppingList(new CreateShoppingListCommand(request.Name, request.ListDate, request.TransactionId, request.Items.Select(x => new CreateShoppingListItemCommand(x.ProductVariantId, x.DesiredQuantity, x.Unit, x.Notes)).ToArray()), ct)));
        group.MapGet("lists/{id:guid}/recommendation", async (Guid id, ShoppingService service, CancellationToken ct) => Results.Ok(await service.Recommend(id, ct)));

        group.MapGet("receipt-imports", async (ShoppingService service, CancellationToken ct) => Results.Ok(await service.ListReceiptImports(ct)));
        group.MapPost("receipt-imports/analyze", AnalyzeReceipt);
        group.MapPost("receipt-imports/{id:guid}/confirm", async (Guid id, ConfirmReceiptImportRequest request, ShoppingService service, CancellationToken ct) =>
            Results.Ok(await service.ConfirmReceiptImport(id, new ConfirmReceiptImportCommand(request.StoreId, request.Lines.Select(x => new ConfirmReceiptImportLineCommand(x.ProductName, x.VariantName, x.Quantity, x.Unit, x.TotalPrice, x.ProductVariantId)).ToArray()), ct)));
    }

    private static async Task<IResult> AnalyzeReceipt(
        HttpContext httpContext,
        ICurrentUser currentUser,
        ITransactionAttachmentProcessor processor,
        ITransactionAttachmentStorage storage,
        IReceiptParser parser,
        ShoppingService service,
        CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new InvalidOperationException("Authenticated user is required.");
        var form = await httpContext.Request.ReadFormAsync(ct);
        var file = form.Files.FirstOrDefault() ?? throw new InvalidOperationException("Receipt image is required.");

        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Receipt file must be an image.");
        }

        await using var prepared = await processor.PrepareAsync(file, ct);
        await using var parseCopy = new MemoryStream();
        await prepared.Content.CopyToAsync(parseCopy, ct);
        parseCopy.Position = 0;
        prepared.Content.Position = 0;

        var importId = Guid.NewGuid();
        var storagePath = await storage.UploadAsync(userId, importId, prepared.FileName, prepared.ContentType, prepared.Content, ct);
        var parsed = await parser.ParseAsync(parseCopy, prepared.ContentType, ct);
        var result = await service.CreateReceiptImport(prepared.FileName, prepared.ContentType, prepared.SizeBytes, storagePath, parsed, ct);
        return Results.Created($"/api/v1/shopping/receipt-imports/{result.Id}", result);
    }
}

public sealed record CreateStoreRequest(string Name, string? Notes);
public sealed record CreateProductRequest(string Name, Guid? CategoryId, string? Notes);
public sealed record CreateProductVariantRequest(Guid ProductId, string Name, decimal NormalizedQuantity, ShoppingUnit Unit, string? Barcode);
public sealed record CreateStoreProductPriceRequest(Guid StoreId, Guid ProductVariantId, decimal TotalPrice, decimal NormalizedQuantity, ShoppingUnit Unit, DateTimeOffset ObservedAt, string? Notes);
public sealed record CreateShoppingListRequest(string Name, DateTimeOffset ListDate, Guid? TransactionId, IReadOnlyList<CreateShoppingListItemRequest> Items);
public sealed record CreateShoppingListItemRequest(Guid ProductVariantId, decimal DesiredQuantity, ShoppingUnit Unit, string? Notes);
public sealed record ConfirmReceiptImportRequest(Guid StoreId, IReadOnlyList<ConfirmReceiptImportLineRequest> Lines);
public sealed record ConfirmReceiptImportLineRequest(string ProductName, string VariantName, decimal Quantity, ShoppingUnit Unit, decimal TotalPrice, Guid? ProductVariantId);
