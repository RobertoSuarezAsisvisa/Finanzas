using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Goals.Handlers;

public sealed class DeleteFinancialGoalContributionHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteFinancialGoalContributionCommand command, CancellationToken cancellationToken = default)
    {
        var contribution = await dbContext.Set<FinancialGoalContribution>()
            .Include(x => x.Goal)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);

        if (contribution.TransactionId is not null)
        {
            var transaction = await dbContext.Set<Transaction>()
                .Include(x => x.Account)
                .Include(x => x.ToAccount)
                .FirstAsync(x => x.Id == contribution.TransactionId, cancellationToken);
            TransactionBalanceAdjustment.Reverse(transaction);
            transaction.SoftDelete();
        }

        contribution.Goal.AdjustContribution(-contribution.Amount);
        contribution.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
