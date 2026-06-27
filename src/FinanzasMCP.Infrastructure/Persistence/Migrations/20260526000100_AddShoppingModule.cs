using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace FinanzasMCP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(FinanzasMCPDbContext))]
    [Migration("20260526000100_AddShoppingModule")]
    public partial class AddShoppingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "shopping_stores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_shopping_stores", x => x.Id));

            migrationBuilder.CreateTable(
                name: "shopping_products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopping_products", x => x.Id);
                    table.ForeignKey("FK_shopping_products_categories_CategoryId", x => x.CategoryId, "categories", "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "shopping_receipt_imports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: true),
                    DetectedStoreName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    ReceiptDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StoragePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopping_receipt_imports", x => x.Id);
                    table.ForeignKey("FK_shopping_receipt_imports_shopping_stores_StoreId", x => x.StoreId, "shopping_stores", "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "shopping_product_variants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    NormalizedQuantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Barcode = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopping_product_variants", x => x.Id);
                    table.ForeignKey("FK_shopping_product_variants_shopping_products_ProductId", x => x.ProductId, "shopping_products", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shopping_lists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    ListDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopping_lists", x => x.Id);
                    table.ForeignKey("FK_shopping_lists_transactions_TransactionId", x => x.TransactionId, "transactions", "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "shopping_store_product_prices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NormalizedQuantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    ObservedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ReceiptImportId = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopping_store_product_prices", x => x.Id);
                    table.ForeignKey("FK_shopping_store_product_prices_shopping_product_variants_ProductVariantId", x => x.ProductVariantId, "shopping_product_variants", "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_shopping_store_product_prices_shopping_receipt_imports_ReceiptImportId", x => x.ReceiptImportId, "shopping_receipt_imports", "Id", onDelete: ReferentialAction.SetNull);
                    table.ForeignKey("FK_shopping_store_product_prices_shopping_stores_StoreId", x => x.StoreId, "shopping_stores", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "shopping_list_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShoppingListId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesiredQuantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopping_list_items", x => x.Id);
                    table.ForeignKey("FK_shopping_list_items_shopping_lists_ShoppingListId", x => x.ShoppingListId, "shopping_lists", "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_shopping_list_items_shopping_product_variants_ProductVariantId", x => x.ProductVariantId, "shopping_product_variants", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "shopping_receipt_import_lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptImportId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    VariantName = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shopping_receipt_import_lines", x => x.Id);
                    table.ForeignKey("FK_shopping_receipt_import_lines_shopping_product_variants_ProductVariantId", x => x.ProductVariantId, "shopping_product_variants", "Id", onDelete: ReferentialAction.SetNull);
                    table.ForeignKey("FK_shopping_receipt_import_lines_shopping_receipt_imports_ReceiptImportId", x => x.ReceiptImportId, "shopping_receipt_imports", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex("IX_shopping_stores_UserId", "shopping_stores", "UserId");
            migrationBuilder.CreateIndex("IX_shopping_stores_UserId_Name", "shopping_stores", new[] { "UserId", "Name" }, unique: true);
            migrationBuilder.CreateIndex("IX_shopping_products_CategoryId", "shopping_products", "CategoryId");
            migrationBuilder.CreateIndex("IX_shopping_products_UserId", "shopping_products", "UserId");
            migrationBuilder.CreateIndex("IX_shopping_products_UserId_Name", "shopping_products", new[] { "UserId", "Name" }, unique: true);
            migrationBuilder.CreateIndex("IX_shopping_product_variants_ProductId", "shopping_product_variants", "ProductId");
            migrationBuilder.CreateIndex("IX_shopping_product_variants_UserId", "shopping_product_variants", "UserId");
            migrationBuilder.CreateIndex("IX_shopping_product_variants_UserId_ProductId_Name", "shopping_product_variants", new[] { "UserId", "ProductId", "Name" }, unique: true);
            migrationBuilder.CreateIndex("IX_shopping_lists_ListDate", "shopping_lists", "ListDate");
            migrationBuilder.CreateIndex("IX_shopping_lists_TransactionId", "shopping_lists", "TransactionId");
            migrationBuilder.CreateIndex("IX_shopping_lists_UserId", "shopping_lists", "UserId");
            migrationBuilder.CreateIndex("IX_shopping_list_items_ProductVariantId", "shopping_list_items", "ProductVariantId");
            migrationBuilder.CreateIndex("IX_shopping_list_items_ShoppingListId", "shopping_list_items", "ShoppingListId");
            migrationBuilder.CreateIndex("IX_shopping_list_items_UserId", "shopping_list_items", "UserId");
            migrationBuilder.CreateIndex("IX_shopping_receipt_imports_StoreId", "shopping_receipt_imports", "StoreId");
            migrationBuilder.CreateIndex("IX_shopping_receipt_imports_UserId", "shopping_receipt_imports", "UserId");
            migrationBuilder.CreateIndex("IX_shopping_receipt_import_lines_ProductVariantId", "shopping_receipt_import_lines", "ProductVariantId");
            migrationBuilder.CreateIndex("IX_shopping_receipt_import_lines_ReceiptImportId", "shopping_receipt_import_lines", "ReceiptImportId");
            migrationBuilder.CreateIndex("IX_shopping_receipt_import_lines_UserId", "shopping_receipt_import_lines", "UserId");
            migrationBuilder.CreateIndex("IX_shopping_store_product_prices_ProductVariantId_StoreId_ObservedAt", "shopping_store_product_prices", new[] { "ProductVariantId", "StoreId", "ObservedAt" });
            migrationBuilder.CreateIndex("IX_shopping_store_product_prices_ReceiptImportId", "shopping_store_product_prices", "ReceiptImportId");
            migrationBuilder.CreateIndex("IX_shopping_store_product_prices_StoreId", "shopping_store_product_prices", "StoreId");
            migrationBuilder.CreateIndex("IX_shopping_store_product_prices_UserId", "shopping_store_product_prices", "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("shopping_list_items");
            migrationBuilder.DropTable("shopping_receipt_import_lines");
            migrationBuilder.DropTable("shopping_store_product_prices");
            migrationBuilder.DropTable("shopping_lists");
            migrationBuilder.DropTable("shopping_product_variants");
            migrationBuilder.DropTable("shopping_receipt_imports");
            migrationBuilder.DropTable("shopping_products");
            migrationBuilder.DropTable("shopping_stores");
        }
    }
}
