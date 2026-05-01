using FinanzasMCP.Application.Accounts.Commands;
using FinanzasMCP.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Accounts.Handlers;

public sealed class DeleteAccountHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteAccountCommand command, CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts.FirstAsync(x => x.Id == command.Id, cancellationToken);
        account.Deactivate();
        account.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
