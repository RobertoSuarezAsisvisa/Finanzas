using FinanzasMCP.Domain.Categories;

namespace FinanzasMCP.Application.Categories.Commands;

public sealed record CreateCategoryCommand(string Name, CategoryType Type, string? Icon, Guid? ParentId, bool IsSystem);
