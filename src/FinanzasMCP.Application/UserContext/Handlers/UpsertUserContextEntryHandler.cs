using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.UserContext.Commands;
using FinanzasMCP.Domain.UserContext;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.UserContext.Handlers;

public sealed class UpsertUserContextEntryHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<UserContextEntrySummary> Handle(UpsertUserContextEntryCommand command, CancellationToken cancellationToken = default)
    {
        var entry = await dbContext.Set<UserContextEntry>().FirstOrDefaultAsync(x => x.Key == command.Key, cancellationToken);
        if (entry is null)
        {
            entry = UserContextEntry.Create(command.Key, command.Value);
            dbContext.Set<UserContextEntry>().Add(entry);
        }
        else
        {
            entry.UpdateValue(command.Value);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UserContextEntrySummary(entry.Id, entry.Key, entry.Value, entry.UpdatedAt);
    }
}
