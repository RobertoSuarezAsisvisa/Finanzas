using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.CreditCards.Services;
using FinanzasMCP.Application.Transactions.Commands;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Transactions.Handlers;

public sealed class DeleteTransactionHandler(IFinanzasMCPDbContext dbContext, CreditCardTransactionService creditCardTransactionService)
{
    public async Task Handle(DeleteTransactionCommand command, CancellationToken cancellationToken = default)
    {
        var transaction = await dbContext.Set<Transaction>()
            .Include(x => x.Account)
            .Include(x => x.ToAccount)
            .Include(x => x.Attachments)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);

        await creditCardTransactionService.ReverseAsync(transaction, cancellationToken);
        foreach (var attachment in transaction.Attachments)
        {
            attachment.SoftDelete();
        }
        transaction.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

}
