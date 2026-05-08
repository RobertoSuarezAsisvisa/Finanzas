using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Queries;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class GetDebtInstallmentsHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<IReadOnlyList<DebtInstallmentSummary>> Handle(GetDebtInstallmentsQuery query, CancellationToken cancellationToken = default)
    {
        var installmentsQuery = dbContext.Set<DebtInstallment>().AsNoTracking().AsQueryable();
        var paymentsQuery = dbContext.Set<DebtPayment>().AsNoTracking().AsQueryable();

        if (query.DebtId is not null)
        {
            installmentsQuery = installmentsQuery.Where(x => x.DebtId == query.DebtId);
            paymentsQuery = paymentsQuery.Where(x => x.DebtId == query.DebtId);
        }

        var installments = await installmentsQuery
            .OrderBy(x => x.DueDate)
            .ThenBy(x => x.Number)
            .ToListAsync(cancellationToken);
        var paidByDebt = (await paymentsQuery
            .GroupBy(x => x.DebtId)
            .Select(x => new { DebtId = x.Key, Paid = x.Sum(p => p.Amount) })
            .ToListAsync(cancellationToken))
            .ToDictionary(x => x.DebtId, x => x.Paid);
        var paidCursorByDebt = new Dictionary<Guid, decimal>();
        var today = DateTimeOffset.UtcNow.Date;

        return installments.Select(installment =>
        {
            var totalPaid = paidByDebt.GetValueOrDefault(installment.DebtId);
            var consumed = paidCursorByDebt.GetValueOrDefault(installment.DebtId);
            var available = Math.Max(0m, totalPaid - consumed);
            var paidAmount = Math.Min(installment.ExpectedPayment, available);
            paidCursorByDebt[installment.DebtId] = consumed + paidAmount;
            var pending = Math.Max(0m, installment.ExpectedPayment - paidAmount);
            var daysOverdue = pending > 0 && installment.DueDate.Date < today
                ? (today - installment.DueDate.Date).Days
                : 0;
            var status = pending <= 0
                ? "Paid"
                : paidAmount > 0
                    ? "Partial"
                    : daysOverdue > 0
                        ? "Overdue"
                        : "Pending";

            return new DebtInstallmentSummary(
                installment.Id,
                installment.DebtId,
                installment.Number,
                installment.DueDate,
                installment.ExpectedPayment,
                installment.Principal,
                installment.Interest,
                paidAmount,
                pending,
                installment.BalanceAfterPayment,
                status,
                daysOverdue);
        }).ToArray();
    }
}
