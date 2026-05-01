using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Categories.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Categories.Handlers;

public sealed class CreateCategoryHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<CategorySummary> Handle(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var category = Category.Create(command.Name, command.Type, command.Icon, command.ParentId, command.IsSystem);
        dbContext.Set<Category>().Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CategorySummary(category.Id, category.Name, category.Type, category.Icon, category.IsSystem, category.ParentId);
    }
}
