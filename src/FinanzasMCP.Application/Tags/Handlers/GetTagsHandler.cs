using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Tags.Queries;
using FinanzasMCP.Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Tags.Handlers;

public sealed class GetTagsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<TagSummary>> Handle(GetTagsQuery query, CancellationToken cancellationToken = default)
    {
        var tags = await dbContext.Set<Tag>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return tags.Select(x => new TagSummary(x.Id, x.Name, x.Color)).ToArray();
    }
}
