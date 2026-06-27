using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Shopping;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Shopping;

public sealed class ShoppingService(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<StoreSummary>> ListStores(CancellationToken cancellationToken = default)
        => await dbContext.Stores.AsNoTracking().OrderBy(x => x.Name).Select(x => new StoreSummary(x.Id, x.Name, x.Notes)).ToArrayAsync(cancellationToken);

    public async Task<StoreSummary> CreateStore(CreateStoreCommand command, CancellationToken cancellationToken = default)
    {
        var store = Store.Create(command.Name, command.Notes);
        dbContext.Stores.Add(store);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new StoreSummary(store.Id, store.Name, store.Notes);
    }

    public async Task<StoreSummary> UpdateStore(UpdateStoreCommand command, CancellationToken cancellationToken = default)
    {
        var store = await dbContext.Stores.FirstAsync(x => x.Id == command.Id, cancellationToken);
        store.UpdateDetails(command.Name, command.Notes);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new StoreSummary(store.Id, store.Name, store.Notes);
    }

    public async Task DeleteStore(Guid id, CancellationToken cancellationToken = default)
    {
        var store = await dbContext.Stores.FirstAsync(x => x.Id == id, cancellationToken);
        store.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductSummary>> ListProducts(CancellationToken cancellationToken = default)
        => await dbContext.Products.AsNoTracking().OrderBy(x => x.Name).Select(x => new ProductSummary(x.Id, x.Name, x.CategoryId, x.Notes)).ToArrayAsync(cancellationToken);

    public async Task<ProductSummary> CreateProduct(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        var product = Product.Create(command.Name, command.CategoryId, command.Notes);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ProductSummary(product.Id, product.Name, product.CategoryId, product.Notes);
    }

    public async Task<ProductSummary> UpdateProduct(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstAsync(x => x.Id == command.Id, cancellationToken);
        product.UpdateDetails(command.Name, command.CategoryId, command.Notes);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ProductSummary(product.Id, product.Name, product.CategoryId, product.Notes);
    }

    public async Task DeleteProduct(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FirstAsync(x => x.Id == id, cancellationToken);
        product.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductVariantSummary>> ListVariants(Guid? productId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.ProductVariants.AsNoTracking().Include(x => x.Product).AsQueryable();
        if (productId is not null)
        {
            query = query.Where(x => x.ProductId == productId);
        }

        return await query.OrderBy(x => x.Product.Name).ThenBy(x => x.Name)
            .Select(x => new ProductVariantSummary(x.Id, x.ProductId, x.Product.Name, x.Name, x.NormalizedQuantity, x.Unit, x.Barcode))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<ProductVariantSummary> CreateVariant(CreateProductVariantCommand command, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.AsNoTracking().FirstAsync(x => x.Id == command.ProductId, cancellationToken);
        var variant = ProductVariant.Create(command.ProductId, command.Name, command.NormalizedQuantity, command.Unit, command.Barcode);
        dbContext.ProductVariants.Add(variant);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ProductVariantSummary(variant.Id, variant.ProductId, product.Name, variant.Name, variant.NormalizedQuantity, variant.Unit, variant.Barcode);
    }

    public async Task<ProductVariantSummary> UpdateVariant(UpdateProductVariantCommand command, CancellationToken cancellationToken = default)
    {
        var variant = await dbContext.ProductVariants.Include(x => x.Product).FirstAsync(x => x.Id == command.Id, cancellationToken);
        variant.UpdateDetails(command.Name, command.NormalizedQuantity, command.Unit, command.Barcode);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ProductVariantSummary(variant.Id, variant.ProductId, variant.Product.Name, variant.Name, variant.NormalizedQuantity, variant.Unit, variant.Barcode);
    }

    public async Task DeleteVariant(Guid id, CancellationToken cancellationToken = default)
    {
        var variant = await dbContext.ProductVariants.FirstAsync(x => x.Id == id, cancellationToken);
        variant.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StoreProductPriceSummary>> ListPrices(Guid? storeId = null, Guid? productVariantId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.StoreProductPrices.AsNoTracking().Include(x => x.Store).Include(x => x.ProductVariant).ThenInclude(x => x.Product).AsQueryable();
        if (storeId is not null)
        {
            query = query.Where(x => x.StoreId == storeId);
        }

        if (productVariantId is not null)
        {
            query = query.Where(x => x.ProductVariantId == productVariantId);
        }

        return await query.OrderByDescending(x => x.ObservedAt).Take(500).Select(x => ToPriceSummary(x)).ToArrayAsync(cancellationToken);
    }

    public async Task<StoreProductPriceSummary> CreatePrice(CreateStoreProductPriceCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureStoreAndVariant(command.StoreId, command.ProductVariantId, cancellationToken);
        var price = StoreProductPrice.Create(command.StoreId, command.ProductVariantId, command.TotalPrice, command.NormalizedQuantity, command.Unit, command.ObservedAt, StoreProductPriceSource.Manual, notes: command.Notes);
        dbContext.StoreProductPrices.Add(price);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetPrice(price.Id, cancellationToken);
    }

    public async Task<StoreProductPriceSummary> UpdatePrice(UpdateStoreProductPriceCommand command, CancellationToken cancellationToken = default)
    {
        var price = await dbContext.StoreProductPrices.FirstAsync(x => x.Id == command.Id, cancellationToken);
        price.UpdateDetails(command.TotalPrice, command.NormalizedQuantity, command.Unit, command.ObservedAt, command.Notes);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetPrice(price.Id, cancellationToken);
    }

    public async Task DeletePrice(Guid id, CancellationToken cancellationToken = default)
    {
        var price = await dbContext.StoreProductPrices.FirstAsync(x => x.Id == id, cancellationToken);
        price.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ShoppingListSummary> CreateShoppingList(CreateShoppingListCommand command, CancellationToken cancellationToken = default)
    {
        var list = ShoppingList.Create(command.Name, command.ListDate, command.TransactionId);
        dbContext.ShoppingLists.Add(list);
        foreach (var item in command.Items)
        {
            await dbContext.ProductVariants.AsNoTracking().FirstAsync(x => x.Id == item.ProductVariantId, cancellationToken);
            dbContext.ShoppingListItems.Add(ShoppingListItem.Create(list.Id, item.ProductVariantId, item.DesiredQuantity, item.Unit, item.Notes));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetShoppingList(list.Id, cancellationToken);
    }

    public async Task<IReadOnlyList<ShoppingListSummary>> ListShoppingLists(CancellationToken cancellationToken = default)
    {
        var lists = await dbContext.ShoppingLists.AsNoTracking().Include(x => x.Items).ThenInclude(x => x.ProductVariant).ThenInclude(x => x.Product)
            .OrderByDescending(x => x.ListDate).Take(100).ToArrayAsync(cancellationToken);
        return lists.Select(ToListSummary).ToArray();
    }

    public async Task<ShoppingRecommendationSummary> Recommend(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        var list = await dbContext.ShoppingLists.AsNoTracking().Include(x => x.Items).ThenInclude(x => x.ProductVariant).ThenInclude(x => x.Product)
            .FirstAsync(x => x.Id == shoppingListId, cancellationToken);

        var variantIds = list.Items.Select(x => x.ProductVariantId).Distinct().ToArray();
        var priceHistory = await dbContext.StoreProductPrices.AsNoTracking().Include(x => x.Store).Include(x => x.ProductVariant).ThenInclude(x => x.Product)
            .Where(x => variantIds.Contains(x.ProductVariantId))
            .ToArrayAsync(cancellationToken);
        var latestPrices = priceHistory
            .GroupBy(x => new { x.StoreId, x.ProductVariantId })
            .Select(g => g.OrderByDescending(x => x.ObservedAt).First())
            .ToArray();

        var groups = new Dictionary<Guid, (string StoreName, List<ShoppingRecommendationItem> Items)>();
        var missing = new List<ShoppingRecommendationMissingItem>();
        decimal estimatedTotal = 0m;
        decimal baselineTotal = 0m;

        foreach (var item in list.Items)
        {
            var prices = latestPrices.Where(x => x.ProductVariantId == item.ProductVariantId).OrderBy(x => x.UnitPrice).ToArray();
            if (prices.Length == 0)
            {
                missing.Add(new ShoppingRecommendationMissingItem(item.ProductVariantId, item.ProductVariant.Product.Name, item.ProductVariant.Name, item.DesiredQuantity, item.Unit));
                continue;
            }

            var best = prices[0];
            var worst = prices[^1];
            var estimatedPrice = Math.Round(best.UnitPrice * item.DesiredQuantity, 2, MidpointRounding.AwayFromZero);
            estimatedTotal += estimatedPrice;
            baselineTotal += Math.Round(worst.UnitPrice * item.DesiredQuantity, 2, MidpointRounding.AwayFromZero);

            if (!groups.TryGetValue(best.StoreId, out var group))
            {
                group = (best.Store.Name, new List<ShoppingRecommendationItem>());
                groups.Add(best.StoreId, group);
            }

            group.Items.Add(new ShoppingRecommendationItem(item.ProductVariantId, item.ProductVariant.Product.Name, item.ProductVariant.Name, item.DesiredQuantity, item.Unit, best.UnitPrice, estimatedPrice, best.ObservedAt));
        }

        var storeGroups = groups.Select(x => new ShoppingRecommendationStoreGroup(
            x.Key,
            x.Value.StoreName,
            x.Value.Items.Sum(item => item.EstimatedPrice),
            x.Value.Items.OrderBy(item => item.ProductName).ThenBy(item => item.VariantName).ToArray()))
            .OrderBy(x => x.StoreName)
            .ToArray();

        return new ShoppingRecommendationSummary(list.Id, estimatedTotal, Math.Max(0, baselineTotal - estimatedTotal), storeGroups, missing);
    }

    public async Task<ReceiptImportSummary> CreateReceiptImport(string fileName, string contentType, long sizeBytes, string storagePath, ParsedReceipt parsedReceipt, CancellationToken cancellationToken = default)
    {
        var receipt = ReceiptImport.Create(fileName, contentType, sizeBytes, storagePath, parsedReceipt.StoreName, parsedReceipt.ReceiptDate);
        dbContext.ReceiptImports.Add(receipt);
        foreach (var line in parsedReceipt.Lines)
        {
            dbContext.ReceiptImportLines.Add(ReceiptImportLine.Create(receipt.Id, line.ProductName, line.VariantName, line.Quantity, line.Unit, line.TotalPrice));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetReceiptImport(receipt.Id, cancellationToken);
    }

    public async Task<IReadOnlyList<ReceiptImportSummary>> ListReceiptImports(CancellationToken cancellationToken = default)
    {
        var receipts = await dbContext.ReceiptImports.AsNoTracking().Include(x => x.Store).Include(x => x.Lines)
            .OrderByDescending(x => x.CreatedAt).Take(100).ToArrayAsync(cancellationToken);
        return receipts.Select(ToReceiptSummary).ToArray();
    }

    public async Task<ReceiptImportSummary> ConfirmReceiptImport(Guid id, ConfirmReceiptImportCommand command, CancellationToken cancellationToken = default)
    {
        var receipt = await dbContext.ReceiptImports.Include(x => x.Lines).FirstAsync(x => x.Id == id, cancellationToken);
        if (receipt.Status != ReceiptImportStatus.PendingReview)
        {
            throw new InvalidOperationException("Receipt import is already finalized.");
        }

        await dbContext.Stores.AsNoTracking().FirstAsync(x => x.Id == command.StoreId, cancellationToken);
        var validLines = command.Lines
            .Where(line => !string.IsNullOrWhiteSpace(line.ProductName) || !string.IsNullOrWhiteSpace(line.VariantName) || line.TotalPrice > 0)
            .ToArray();

        if (validLines.Length == 0)
        {
            throw new InvalidOperationException("At least one reviewed receipt line is required.");
        }

        foreach (var line in validLines)
        {
            if (string.IsNullOrWhiteSpace(line.ProductName) || string.IsNullOrWhiteSpace(line.VariantName))
            {
                throw new InvalidOperationException("Receipt product and variant names are required.");
            }

            if (line.Quantity <= 0 || line.TotalPrice <= 0)
            {
                throw new InvalidOperationException("Receipt line quantity and total price must be positive.");
            }
        }

        foreach (var line in validLines)
        {
            var variantId = line.ProductVariantId ?? await CreateProductAndVariant(line, cancellationToken);
            dbContext.StoreProductPrices.Add(StoreProductPrice.Create(command.StoreId, variantId, line.TotalPrice, line.Quantity, line.Unit, receipt.ReceiptDate ?? DateTimeOffset.UtcNow, StoreProductPriceSource.ReceiptImage, receipt.Id));
        }

        receipt.Confirm(command.StoreId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetReceiptImport(receipt.Id, cancellationToken);
    }

    private async Task<Guid> CreateProductAndVariant(ConfirmReceiptImportLineCommand line, CancellationToken cancellationToken)
    {
        var productName = line.ProductName.Trim();
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Name.ToLower() == productName.ToLower(), cancellationToken);
        if (product is null)
        {
            product = Product.Create(productName);
            dbContext.Products.Add(product);
        }

        var variantName = line.VariantName.Trim();
        var variant = await dbContext.ProductVariants.FirstOrDefaultAsync(x => x.ProductId == product.Id && x.Name.ToLower() == variantName.ToLower(), cancellationToken);
        if (variant is not null)
        {
            return variant.Id;
        }

        variant = ProductVariant.Create(product.Id, variantName, line.Quantity, line.Unit);
        dbContext.ProductVariants.Add(variant);
        return variant.Id;
    }

    private async Task EnsureStoreAndVariant(Guid storeId, Guid productVariantId, CancellationToken cancellationToken)
    {
        await dbContext.Stores.AsNoTracking().FirstAsync(x => x.Id == storeId, cancellationToken);
        await dbContext.ProductVariants.AsNoTracking().FirstAsync(x => x.Id == productVariantId, cancellationToken);
    }

    private async Task<StoreProductPriceSummary> GetPrice(Guid id, CancellationToken cancellationToken)
    {
        var price = await dbContext.StoreProductPrices.AsNoTracking().Include(x => x.Store).Include(x => x.ProductVariant).ThenInclude(x => x.Product).FirstAsync(x => x.Id == id, cancellationToken);
        return ToPriceSummary(price);
    }

    private async Task<ShoppingListSummary> GetShoppingList(Guid id, CancellationToken cancellationToken)
    {
        var list = await dbContext.ShoppingLists.AsNoTracking().Include(x => x.Items).ThenInclude(x => x.ProductVariant).ThenInclude(x => x.Product).FirstAsync(x => x.Id == id, cancellationToken);
        return ToListSummary(list);
    }

    private async Task<ReceiptImportSummary> GetReceiptImport(Guid id, CancellationToken cancellationToken)
    {
        var receipt = await dbContext.ReceiptImports.AsNoTracking().Include(x => x.Store).Include(x => x.Lines).FirstAsync(x => x.Id == id, cancellationToken);
        return ToReceiptSummary(receipt);
    }

    private static StoreProductPriceSummary ToPriceSummary(StoreProductPrice x)
        => new(x.Id, x.StoreId, x.Store.Name, x.ProductVariantId, x.ProductVariant.Product.Name, x.ProductVariant.Name, x.TotalPrice, x.NormalizedQuantity, x.Unit, x.UnitPrice, x.ObservedAt, x.Source, x.ReceiptImportId, x.Notes);

    private static ShoppingListSummary ToListSummary(ShoppingList x)
        => new(x.Id, x.Name, x.ListDate, x.TransactionId, x.Items.OrderBy(item => item.ProductVariant.Product.Name).Select(item => new ShoppingListItemSummary(item.Id, item.ProductVariantId, item.ProductVariant.Product.Name, item.ProductVariant.Name, item.DesiredQuantity, item.Unit, item.Notes)).ToArray());

    private static ReceiptImportSummary ToReceiptSummary(ReceiptImport x)
        => new(x.Id, x.StoreId, x.Store?.Name, x.DetectedStoreName, x.ReceiptDate, x.FileName, x.ContentType, x.SizeBytes, x.Status, x.Lines.Select(line => new ReceiptImportLineSummary(line.Id, line.ProductName, line.VariantName, line.Quantity, line.Unit, line.TotalPrice, line.UnitPrice, line.ProductVariantId)).ToArray());
}
