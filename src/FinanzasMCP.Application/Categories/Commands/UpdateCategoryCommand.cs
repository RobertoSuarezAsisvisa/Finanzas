using FinanzasMCP.Domain.Categories;

namespace FinanzasMCP.Application.Categories.Commands;

public sealed record UpdateCategoryCommand(Guid Id, string Name, CategoryType Type, string? Icon, Guid? ParentId);
