using FinanzasMCP.Application.Categories.Commands;
using FinanzasMCP.Application.Categories.Handlers;
using FinanzasMCP.Application.Categories.Queries;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Domain.Categories;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class CategoryTools(
    CreateCategoryHandler createCategoryHandler,
    GetCategoriesHandler getCategoriesHandler,
    UpdateCategoryHandler updateCategoryHandler,
    DeleteCategoryHandler deleteCategoryHandler)
{
    [McpServerTool, System.ComponentModel.Description("Creates a financial category.")]
    public Task<CategorySummary> CreateCategory(string name, CategoryType type, string? icon = null, Guid? parentId = null, bool isSystem = false, CancellationToken cancellationToken = default)
        => createCategoryHandler.Handle(new CreateCategoryCommand(name, type, icon, parentId, isSystem), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists financial categories.")]
    public Task<IReadOnlyList<CategorySummary>> ListCategories(CancellationToken cancellationToken = default)
        => getCategoriesHandler.Handle(new GetCategoriesQuery(), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a financial category.")]
    public Task<CategorySummary> UpdateCategory(Guid id, string name, CategoryType type, string? icon = null, Guid? parentId = null, CancellationToken cancellationToken = default)
        => updateCategoryHandler.Handle(new UpdateCategoryCommand(id, name, type, icon, parentId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a financial category.")]
    public Task DeleteCategory(Guid id, CancellationToken cancellationToken = default)
        => deleteCategoryHandler.Handle(new DeleteCategoryCommand(id), cancellationToken);
}
