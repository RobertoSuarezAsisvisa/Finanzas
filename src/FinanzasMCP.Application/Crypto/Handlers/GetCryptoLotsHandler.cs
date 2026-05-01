using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Crypto.Queries;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Crypto;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Crypto.Handlers;

public sealed class GetCryptoLotsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<CryptoLotSummary>> Handle(GetCryptoLotsQuery query, CancellationToken cancellationToken = default)
    {
        var cryptoLots = dbContext.Set<CryptoLot>().AsQueryable();
        if (query.AccountId is not null)
        {
            cryptoLots = cryptoLots.Where(x => x.AccountId == query.AccountId);
        }

        var items = await cryptoLots
            .OrderByDescending(x => x.OperationDate)
            .Select(cryptoLot => new CryptoLotSummary(
                cryptoLot.Id,
                cryptoLot.AccountId,
                cryptoLot.TransactionId,
                cryptoLot.Quantity,
                cryptoLot.BuyPriceUsd,
                cryptoLot.SellPriceUsd,
                cryptoLot.Status,
                cryptoLot.OperationDate))
            .ToListAsync(cancellationToken);

        return items;
    }
}
