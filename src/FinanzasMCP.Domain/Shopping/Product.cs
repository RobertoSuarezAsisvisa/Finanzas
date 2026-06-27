using FinanzasMCP.Domain.Categories;
using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Shopping;

public sealed class Product : UserOwnedEntity
{
    public string Name { get; private set; } = string.Empty;
    public Guid? CategoryId { get; private set; }
    public string? Notes { get; private set; }
    public Category? Category { get; private set; }
    public ICollection<ProductVariant> Variants { get; private set; } = new List<ProductVariant>();

    public static Product Create(string name, Guid? categoryId = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Product name is required.");
        }

        return new Product
        {
            Name = name.Trim(),
            CategoryId = categoryId,
            Notes = notes?.Trim()
        };
    }

    public void UpdateDetails(string name, Guid? categoryId = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Product name is required.");
        }

        Name = name.Trim();
        CategoryId = categoryId;
        Notes = notes?.Trim();
        MarkUpdated();
    }
}
