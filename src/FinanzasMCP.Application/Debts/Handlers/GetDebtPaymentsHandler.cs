using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Queries;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using FinanzasMCP.Domain.Transactions;
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
        var transactionIds = list
            .Where(x => x.TransactionId is not null)
            .Select(x => x.TransactionId!.Value)
            .ToArray();
        var transactionAccounts = await dbContext.Set<Transaction>()
            .AsNoTracking()
            .Where(x => transactionIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => (Guid?)x.AccountId, cancellationToken);

        return list
            .Select(x => new DebtPaymentSummary(
                x.Id,
                x.DebtId,
                x.TransactionId,
                x.TransactionId is not null && transactionAccounts.TryGetValue(x.TransactionId.Value, out var accountId) ? accountId : null,
                x.Amount,
                x.PaymentDate,
                x.Notes))
            .ToArray();
    }
}
