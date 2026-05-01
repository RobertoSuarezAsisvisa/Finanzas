using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class CreateDebtHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<DebtSummary> Handle(CreateDebtCommand command, CancellationToken cancellationToken = default)
    {
        var debt = Debt.Create(command.Type, command.ContactName, command.OriginalAmount, command.RemainingAmount, command.Currency, command.DueDate, command.AccountId, command.Notes);
        dbContext.Set<Debt>().Add(debt);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DebtSummary(debt.Id, debt.Type, debt.ContactName, debt.OriginalAmount, debt.RemainingAmount, debt.Currency, debt.DueDate, debt.AccountId, debt.Status, debt.Notes);
    }
}
