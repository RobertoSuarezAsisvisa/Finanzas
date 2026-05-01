using FinanzasMCP.Application.Crypto.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Crypto;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Crypto.Handlers;

public sealed class DeleteCryptoLotHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteCryptoLotCommand command, CancellationToken cancellationToken = default)
    {
        var cryptoLot = await dbContext.Set<CryptoLot>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        cryptoLot.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
