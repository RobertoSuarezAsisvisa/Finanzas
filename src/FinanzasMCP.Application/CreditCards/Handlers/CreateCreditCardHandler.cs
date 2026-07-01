using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.CreditCards.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.CreditCards;

namespace FinanzasMCP.Application.CreditCards.Handlers;

public sealed class CreateCreditCardHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<CreditCardSummary> Handle(CreateCreditCardCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name)) throw new InvalidOperationException("Account name is required.");
        if (string.IsNullOrWhiteSpace(command.Issuer)) throw new InvalidOperationException("Issuer is required.");

        var account = Account.Create(
            command.Name,
            AccountType.CreditCard,
            command.Currency,
            AccountPurpose.Spending,
            0m,
            command.Issuer,
            command.LastFour,
            command.Brand.ToString());

        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);

        var creditCard = CreditCardAccount.Create(
            account.Id,
            command.Issuer,
            command.Brand,
            command.CreditLimit,
            command.StatementClosingDay,
            command.PaymentDueDay,
            command.ProductName,
            command.LastFour,
            command.OutstandingBalance,
            command.PaymentMode,
            command.RewardsProgram,
            command.StatementDelivery,
            command.InterestNominalAnnual,
            command.InterestEffectiveAnnual);

        dbContext.CreditCardAccounts.Add(creditCard);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(creditCard).Reference(x => x.Account).LoadAsync(cancellationToken);
        return CreditCardMapping.ToSummary(creditCard);
    }
}
