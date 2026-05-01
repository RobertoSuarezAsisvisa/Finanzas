using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Categories.Queries;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Categories.Handlers;

public sealed class GetCategoriesHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<CategorySummary>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken = default)
    {
        var categories = await dbContext.Set<Category>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return categories
            .Select(x => new CategorySummary(x.Id, x.Name, x.Type, x.Icon, x.IsSystem, x.ParentId))
            .ToArray();
    }
}
