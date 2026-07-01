using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Accounts.Queries;
using FinanzasMCP.Application.CreditCards.Handlers;
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
            .Include(x => x.CreditCardAccount)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return accounts.Select(CreditCardMapping.ToAccountSummary).ToArray();
    }
}
