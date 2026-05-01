using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class RegisterDebtPaymentHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<DebtSummary> Handle(RegisterDebtPaymentCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new InvalidOperationException("Amount must be positive.");
        }

        var debt = await dbContext.Set<Debt>().FirstAsync(x => x.Id == command.DebtId, cancellationToken);
        debt.RegisterPayment(command.Amount);
        dbContext.Set<DebtPayment>().Add(DebtPayment.Create(debt.Id, command.Amount, command.PaymentDate.ToUtcSafe(), command.Notes, command.TransactionId));
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DebtSummary(debt.Id, debt.Type, debt.ContactName, debt.OriginalAmount, debt.RemainingAmount, debt.Currency, debt.DueDate, debt.AccountId, debt.Status, debt.Notes);
    }
}
