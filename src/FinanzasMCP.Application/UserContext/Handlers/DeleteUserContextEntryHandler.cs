using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.UserContext.Commands;
using FinanzasMCP.Domain.UserContext;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.UserContext.Handlers;

public sealed class DeleteUserContextEntryHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteUserContextEntryCommand command, CancellationToken cancellationToken = default)
    {
        var entry = await dbContext.Set<UserContextEntry>().FirstAsync(x => x.Key == command.Key, cancellationToken);
        entry.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
