using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.CreditCards.Queries;
using FinanzasMCP.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.CreditCards.Handlers;

public sealed class GetCreditCardsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<CreditCardSummary>> Handle(GetCreditCardsQuery query, CancellationToken cancellationToken = default)
    {
        var cards = await dbContext.CreditCardAccounts
            .AsNoTracking()
            .Include(x => x.Account)
            .Include(x => x.Statements)
            .OrderBy(x => x.Account.Name)
            .ToListAsync(cancellationToken);

        return cards.Select(CreditCardMapping.ToSummary).ToArray();
    }
}
