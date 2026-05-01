using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Categories;

public sealed class Category : SoftDeletableEntity
{
    public string Name { get; private set; } = string.Empty;
    public CategoryType Type { get; private set; }
    public string? Icon { get; private set; }
    public bool IsSystem { get; private set; }
    public Guid? ParentId { get; private set; }
    public Category? Parent { get; private set; }
    public ICollection<Category> Children { get; private set; } = new List<Category>();

    public static Category Create(string name, CategoryType type, string? icon = null, Guid? parentId = null, bool isSystem = false)
        => new()
        {
            Name = name.Trim(),
            Type = type,
            Icon = icon?.Trim(),
            ParentId = parentId,
            IsSystem = isSystem
        };

    public void UpdateDetails(string name, CategoryType type, string? icon = null, Guid? parentId = null)
    {
        Name = name.Trim();
        Type = type;
        Icon = icon?.Trim();
        ParentId = parentId;
        MarkUpdated();
    }
}
