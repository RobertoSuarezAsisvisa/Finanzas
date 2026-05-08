using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Debts.Services;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class CreateDebtHandler(IFinanzasMCPDbContext dbContext, DebtInstallmentScheduleService scheduleService)
{
    public async Task<DebtSummary> Handle(CreateDebtCommand command, CancellationToken cancellationToken = default)
    {
        var debt = Debt.Create(command.Type, command.ContactName, command.OriginalAmount, command.RemainingAmount, command.Currency, command.DueDate, command.AccountId, command.Notes, command.InterestRate, command.InterestPeriod, command.AmortizationMethod, command.TermMonths, command.LoanStartDate);
        dbContext.Set<Debt>().Add(debt);
        foreach (var installment in scheduleService.Build(debt))
        {
            dbContext.Set<DebtInstallment>().Add(installment);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DebtSummary(debt.Id, debt.Type, debt.ContactName, debt.OriginalAmount, debt.RemainingAmount, debt.Currency, debt.DueDate, debt.AccountId, debt.Status, debt.Notes, debt.InterestRate, debt.InterestPeriod, debt.AmortizationMethod, debt.TermMonths, debt.LoanStartDate);
    }
}
