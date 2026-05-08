using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Debts.Services;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class UpdateDebtHandler(IFinanzasMCPDbContext dbContext, DebtInstallmentScheduleService scheduleService)
{
    public async Task<DebtSummary> Handle(UpdateDebtCommand command, CancellationToken cancellationToken = default)
    {
        var debt = await dbContext.Set<Debt>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        debt.UpdateDetails(command.Type, command.ContactName, command.OriginalAmount, command.RemainingAmount, command.Currency, command.DueDate, command.AccountId, command.Status, command.Notes, command.InterestRate, command.InterestPeriod, command.AmortizationMethod, command.TermMonths, command.LoanStartDate);
        var existingInstallments = await dbContext.Set<DebtInstallment>().Where(x => x.DebtId == debt.Id).ToListAsync(cancellationToken);
        dbContext.Set<DebtInstallment>().RemoveRange(existingInstallments);
        foreach (var installment in scheduleService.Build(debt))
        {
            dbContext.Set<DebtInstallment>().Add(installment);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DebtSummary(debt.Id, debt.Type, debt.ContactName, debt.OriginalAmount, debt.RemainingAmount, debt.Currency, debt.DueDate, debt.AccountId, debt.Status, debt.Notes, debt.InterestRate, debt.InterestPeriod, debt.AmortizationMethod, debt.TermMonths, debt.LoanStartDate);
    }
}
