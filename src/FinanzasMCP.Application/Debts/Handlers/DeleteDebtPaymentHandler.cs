using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Debts;
using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Debts.Handlers;

public sealed class DeleteDebtPaymentHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteDebtPaymentCommand command, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Set<DebtPayment>()
            .Include(x => x.Debt)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);
        if (payment.TransactionId is not null)
        {
            var transaction = await dbContext.Set<Transaction>()
                .Include(x => x.Account)
                .FirstAsync(x => x.Id == payment.TransactionId, cancellationToken);
            Reverse(transaction);
            transaction.SoftDelete();
        }

        payment.Debt.AdjustPayment(-payment.Amount);
        payment.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void Reverse(Transaction transaction)
    {
        switch (transaction.Type)
        {
            case TransactionType.Income:
                transaction.Account.Withdraw(transaction.Amount);
                break;
            case TransactionType.Expense:
                transaction.Account.Deposit(transaction.Amount);
                break;
            case TransactionType.Transfer:
                transaction.Account.Deposit(transaction.Amount);
                break;
        }
    }
}
