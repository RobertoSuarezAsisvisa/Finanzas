using FinanzasMCP.Application.CreditCards.Commands;
using FinanzasMCP.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.CreditCards.Handlers;

public sealed class DeleteCreditCardHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteCreditCardCommand command, CancellationToken cancellationToken = default)
    {
        var creditCard = await dbContext.CreditCardAccounts
            .Include(x => x.Account)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);

        if (creditCard.OutstandingBalance > 0)
        {
            throw new InvalidOperationException("Credit card cannot be deleted while it has an outstanding balance.");
        }

        creditCard.UpdateDetails(
            creditCard.Issuer,
            creditCard.Brand,
            creditCard.CreditLimit,
            creditCard.StatementClosingDay,
            creditCard.PaymentDueDay,
            creditCard.ProductName,
            creditCard.LastFour,
            creditCard.PaymentMode,
            creditCard.RewardsProgram,
            creditCard.StatementDelivery,
            creditCard.InterestNominalAnnual,
            creditCard.InterestEffectiveAnnual,
            false);

        creditCard.SoftDelete();
        creditCard.Account.Deactivate();
        creditCard.Account.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
