using FinanzasMCP.Application.CryptoAccounts.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.CryptoAccounts.Handlers;

public sealed class DeleteCryptoAccountHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteCryptoAccountCommand command, CancellationToken cancellationToken = default)
    {
        var cryptoAccount = await dbContext.CryptoAccounts.FirstAsync(x => x.Id == command.Id, cancellationToken);
        cryptoAccount.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
