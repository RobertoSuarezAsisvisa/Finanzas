using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Common;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class UpdateDebtPaymentHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(UpdateDebtPaymentCommand command, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Set<DebtPayment>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        payment.UpdateDetails(command.Amount, command.PaymentDate.ToUtcSafe(), command.Notes, command.TransactionId);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
