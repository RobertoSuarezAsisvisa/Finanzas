using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.UserContext.Queries;
using FinanzasMCP.Domain.UserContext;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.UserContext.Handlers;

public sealed class GetUserContextHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<UserContextEntrySummary>> Handle(GetUserContextQuery query, CancellationToken cancellationToken = default)
    {
        var entries = await dbContext.Set<UserContextEntry>().AsNoTracking().OrderBy(x => x.Key).ToListAsync(cancellationToken);
        return entries.Select(x => new UserContextEntrySummary(x.Id, x.Key, x.Value, x.UpdatedAt)).ToArray();
    }
}
