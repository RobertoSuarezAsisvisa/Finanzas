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
        var cryptoAccount = account.AccountType == Domain.Accounts.AccountType.Crypto ? account.CryptoAccount : null;

        return new AccountSummary(
            account.Id,
            account.Name,
            account.AccountType,
            account.Currency,
            account.Purpose,
            account.Balance,
            account.IsActive,
            account.BankName,
            account.AccountNumber,
            account.Provider,
            cryptoAccount?.Symbol,
            cryptoAccount?.Network,
            cryptoAccount?.Quantity,
            cryptoAccount?.AvgBuyPriceUsd);
    }
}
