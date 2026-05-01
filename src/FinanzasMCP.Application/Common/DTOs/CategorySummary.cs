using FinanzasMCP.Domain.Categories;

namespace FinanzasMCP.Application.Common.DTOs;

public sealed record CategorySummary(Guid Id, string Name, CategoryType Type, string? Icon, bool IsSystem, Guid? ParentId);
