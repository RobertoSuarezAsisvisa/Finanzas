using FinanzasMCP.Application.Auth.Queries;
using FinanzasMCP.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Auth.Handlers;

public sealed class GetUserApiKeysHandler(
    IFinanzasMCPDbContext dbContext,
    ICurrentUser currentUser)
{
    public async Task<IReadOnlyList<UserApiKeySummary>> Handle(GetUserApiKeysQuery query, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.UserId ?? throw new InvalidOperationException("An authenticated user is required.");
        return await dbContext.UserApiKeys
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new UserApiKeySummary(
                x.Id,
                x.Name,
                $"fmcp_{x.LookupKey}_••••••••",
                x.CreatedAt,
                x.LastUsedAt,
                x.RevokedAt,
                x.RevokedAt != null))
            .ToListAsync(cancellationToken);
    }
}
