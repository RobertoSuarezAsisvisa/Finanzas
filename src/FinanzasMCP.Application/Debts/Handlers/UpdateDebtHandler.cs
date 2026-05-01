using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class UpdateDebtHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<DebtSummary> Handle(UpdateDebtCommand command, CancellationToken cancellationToken = default)
    {
        var debt = await dbContext.Set<Debt>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        debt.UpdateDetails(command.Type, command.ContactName, command.OriginalAmount, command.RemainingAmount, command.Currency, command.DueDate, command.AccountId, command.Status, command.Notes);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DebtSummary(debt.Id, debt.Type, debt.ContactName, debt.OriginalAmount, debt.RemainingAmount, debt.Currency, debt.DueDate, debt.AccountId, debt.Status, debt.Notes);
    }
}
