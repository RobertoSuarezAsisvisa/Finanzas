using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.CreditCards.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.CreditCards.Handlers;

public sealed class UpdateCreditCardHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<CreditCardSummary> Handle(UpdateCreditCardCommand command, CancellationToken cancellationToken = default)
    {
        var creditCard = await dbContext.CreditCardAccounts
            .Include(x => x.Account)
            .Include(x => x.Statements)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);

        creditCard.Account.UpdateDetails(
            command.Name,
            AccountType.CreditCard,
            command.Currency,
            AccountPurpose.Spending,
            0m,
            command.Issuer,
            command.LastFour,
            command.Brand.ToString());

        if (command.IsActive)
        {
            creditCard.Account.Activate();
        }
        else
        {
            creditCard.Account.Deactivate();
        }

        creditCard.UpdateDetails(
            command.Issuer,
            command.Brand,
            command.CreditLimit,
            command.StatementClosingDay,
            command.PaymentDueDay,
            command.ProductName,
            command.LastFour,
            command.PaymentMode,
            command.RewardsProgram,
            command.StatementDelivery,
            command.InterestNominalAnnual,
            command.InterestEffectiveAnnual,
            command.IsActive);

        await dbContext.SaveChangesAsync(cancellationToken);
        return CreditCardMapping.ToSummary(creditCard);
    }
}
