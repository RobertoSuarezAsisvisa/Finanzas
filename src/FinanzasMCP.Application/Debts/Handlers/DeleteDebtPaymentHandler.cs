using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class DeleteDebtPaymentHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteDebtPaymentCommand command, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Set<DebtPayment>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        payment.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
