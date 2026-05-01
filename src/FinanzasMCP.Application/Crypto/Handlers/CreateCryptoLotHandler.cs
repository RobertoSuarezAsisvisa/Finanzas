using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Crypto.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Crypto;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Crypto.Handlers;

public sealed class CreateCryptoLotHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<CryptoLotSummary> Handle(CreateCryptoLotCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Quantity <= 0)
        {
            throw new InvalidOperationException("Quantity must be positive.");
        }

        if (command.BuyPriceUsd <= 0)
        {
            throw new InvalidOperationException("Buy price must be positive.");
        }

        await dbContext.Accounts.FirstAsync(x => x.Id == command.AccountId, cancellationToken);

        var cryptoLot = CryptoLot.Create(
            command.AccountId,
            command.TransactionId,
            command.Quantity,
            command.BuyPriceUsd,
            command.SellPriceUsd,
            command.Status,
            command.OperationDate.ToUtcSafe());

        dbContext.Set<CryptoLot>().Add(cryptoLot);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CryptoLotSummary(
            cryptoLot.Id,
            cryptoLot.AccountId,
            cryptoLot.TransactionId,
            cryptoLot.Quantity,
            cryptoLot.BuyPriceUsd,
            cryptoLot.SellPriceUsd,
            cryptoLot.Status,
            cryptoLot.OperationDate);
    }
}
