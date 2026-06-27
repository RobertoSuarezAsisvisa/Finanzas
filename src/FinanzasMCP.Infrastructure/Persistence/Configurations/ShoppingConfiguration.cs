using FinanzasMCP.Domain.Shopping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("shopping_stores");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(160).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.Name }).IsUnique();
    }
}

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("shopping_products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => new { x.UserId, x.Name }).IsUnique();
    }
}

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("shopping_product_variants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(220).IsRequired();
        builder.Property(x => x.NormalizedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.Unit).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Barcode).HasMaxLength(80);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasOne(x => x.Product).WithMany(x => x.Variants).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.UserId, x.ProductId, x.Name }).IsUnique();
    }
}

public sealed class StoreProductPriceConfiguration : IEntityTypeConfiguration<StoreProductPrice>
{
    public void Configure(EntityTypeBuilder<StoreProductPrice> builder)
    {
        builder.ToTable("shopping_store_product_prices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TotalPrice).HasPrecision(18, 2);
        builder.Property(x => x.NormalizedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 6);
        builder.Property(x => x.Unit).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Source).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.ObservedAt).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ProductVariant).WithMany(x => x.Prices).HasForeignKey(x => x.ProductVariantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ReceiptImport).WithMany().HasForeignKey(x => x.ReceiptImportId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => new { x.ProductVariantId, x.StoreId, x.ObservedAt });
    }
}

public sealed class ShoppingListConfiguration : IEntityTypeConfiguration<ShoppingList>
{
    public void Configure(EntityTypeBuilder<ShoppingList> builder)
    {
        builder.ToTable("shopping_lists");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(180).IsRequired();
        builder.Property(x => x.ListDate).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasOne(x => x.Transaction).WithMany().HasForeignKey(x => x.TransactionId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => x.ListDate);
    }
}

public sealed class ShoppingListItemConfiguration : IEntityTypeConfiguration<ShoppingListItem>
{
    public void Configure(EntityTypeBuilder<ShoppingListItem> builder)
    {
        builder.ToTable("shopping_list_items");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DesiredQuantity).HasPrecision(18, 4);
        builder.Property(x => x.Unit).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasOne(x => x.ShoppingList).WithMany(x => x.Items).HasForeignKey(x => x.ShoppingListId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ProductVariant).WithMany().HasForeignKey(x => x.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ReceiptImportConfiguration : IEntityTypeConfiguration<ReceiptImport>
{
    public void Configure(EntityTypeBuilder<ReceiptImport> builder)
    {
        builder.ToTable("shopping_receipt_imports");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DetectedStoreName).HasMaxLength(160);
        builder.Property(x => x.FileName).HasMaxLength(260).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.StoragePath).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasOne(x => x.Store).WithMany().HasForeignKey(x => x.StoreId).OnDelete(DeleteBehavior.SetNull);
    }
}

public sealed class ReceiptImportLineConfiguration : IEntityTypeConfiguration<ReceiptImportLine>
{
    public void Configure(EntityTypeBuilder<ReceiptImportLine> builder)
    {
        builder.ToTable("shopping_receipt_import_lines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.VariantName).HasMaxLength(220).IsRequired();
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.TotalPrice).HasPrecision(18, 2);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 6);
        builder.Property(x => x.Unit).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasOne(x => x.ReceiptImport).WithMany(x => x.Lines).HasForeignKey(x => x.ReceiptImportId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.ProductVariant).WithMany().HasForeignKey(x => x.ProductVariantId).OnDelete(DeleteBehavior.SetNull);
    }
}
