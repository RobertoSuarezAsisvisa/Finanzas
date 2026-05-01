using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Queries;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class GetDebtPaymentsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<DebtPaymentSummary>> Handle(GetDebtPaymentsQuery query, CancellationToken cancellationToken = default)
    {
        var payments = dbContext.Set<DebtPayment>().AsNoTracking().AsQueryable();
        if (query.DebtId is not null)
        {
            payments = payments.Where(x => x.DebtId == query.DebtId);
        }

        var list = await payments.OrderByDescending(x => x.PaymentDate).ToListAsync(cancellationToken);
        return list.Select(x => new DebtPaymentSummary(x.Id, x.DebtId, x.TransactionId, x.Amount, x.PaymentDate, x.Notes)).ToArray();
    }
}
