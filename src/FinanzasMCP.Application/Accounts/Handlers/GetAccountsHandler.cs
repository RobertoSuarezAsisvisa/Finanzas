using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Accounts.Queries;
using FinanzasMCP.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Accounts.Handlers;

public sealed class GetAccountsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<AccountSummary>> Handle(GetAccountsQuery query, CancellationToken cancellationToken = default)
    {
        var accounts = await dbContext.Accounts
            .AsNoTracking()
            .Include(x => x.CryptoAccount)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return accounts.Select(Map).ToArray();
    }

    private static AccountSummary Map(Domain.Accounts.Account account)
    {
        return new AccountSummary(
            account.Id,
            account.Name,
            account.AccountType,
            account.Currency,
            account.Balance,
            account.IsActive,
            account.BankName,
            account.Provider,
            account.CryptoAccount?.Symbol,
            account.CryptoAccount?.Network,
            account.CryptoAccount?.Quantity,
            account.CryptoAccount?.AvgBuyPriceUsd);
    }
}
