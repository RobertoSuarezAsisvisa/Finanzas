using FinanzasMCP.Application.Categories.Commands;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Categories.Handlers;

public sealed class UpdateCategoryHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<CategorySummary> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var category = await dbContext.Set<Category>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        category.UpdateDetails(command.Name, command.Type, command.Icon, command.ParentId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CategorySummary(category.Id, category.Name, category.Type, category.Icon, category.IsSystem, category.ParentId);
    }
}
