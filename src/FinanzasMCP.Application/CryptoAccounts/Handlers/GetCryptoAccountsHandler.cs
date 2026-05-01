using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.CryptoAccounts.Queries;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.CryptoAccounts.Handlers;

public sealed class GetCryptoAccountsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<CryptoAccountSummary>> Handle(GetCryptoAccountsQuery query, CancellationToken cancellationToken = default)
    {
        var cryptoAccounts = dbContext.CryptoAccounts.AsQueryable();
        if (query.AccountId is not null)
        {
            cryptoAccounts = cryptoAccounts.Where(x => x.AccountId == query.AccountId);
        }

        return await cryptoAccounts
            .OrderBy(x => x.Symbol)
            .Select(cryptoAccount => new CryptoAccountSummary(
                cryptoAccount.Id,
                cryptoAccount.AccountId,
                cryptoAccount.Symbol,
                cryptoAccount.Network,
                cryptoAccount.Quantity,
                cryptoAccount.AvgBuyPriceUsd))
            .ToListAsync(cancellationToken);
    }
}
