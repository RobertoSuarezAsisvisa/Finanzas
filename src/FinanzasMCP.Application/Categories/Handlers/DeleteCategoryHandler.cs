using FinanzasMCP.Application.Categories.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Categories.Handlers;

public sealed class DeleteCategoryHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var category = await dbContext.Set<Category>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        category.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
