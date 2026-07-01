using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.CreditCards.Queries;
using FinanzasMCP.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.CreditCards.Handlers;

public sealed class GetCreditCardStatementsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<CreditCardStatementSummary>> Handle(GetCreditCardStatementsQuery query, CancellationToken cancellationToken = default)
    {
        var statements = dbContext.CreditCardStatements.AsNoTracking().AsQueryable();
        if (query.CreditCardId is not null)
        {
            statements = statements.Where(x => x.CreditCardAccountId == query.CreditCardId.Value);
        }

        var rows = await statements.OrderByDescending(x => x.StatementDate).ToListAsync(cancellationToken);
        return rows.Select(CreditCardMapping.ToStatementSummary).ToArray();
    }
}
