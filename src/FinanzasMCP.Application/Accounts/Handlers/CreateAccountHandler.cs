using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Accounts.Commands;
using FinanzasMCP.Application.CreditCards.Handlers;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.CreditCards;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Accounts.Handlers;

public sealed class CreateAccountHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<AccountSummary> Handle(CreateAccountCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new InvalidOperationException("Account name is required.");
        }

        if (command.AccountType == AccountType.Crypto && string.IsNullOrWhiteSpace(command.CryptoSymbol))
        {
            throw new InvalidOperationException("Crypto symbol is required for crypto accounts.");
        }

        if (command.AccountType == AccountType.CreditCard && string.IsNullOrWhiteSpace(command.CreditCardIssuer))
        {
            throw new InvalidOperationException("Issuer is required for credit card accounts.");
        }

        var account = Account.Create(
            command.Name,
            command.AccountType,
            command.Currency,
            command.Purpose,
            command.AccountType == AccountType.CreditCard ? 0m : command.Balance,
            command.BankName,
            command.AccountNumber,
            command.Provider);

        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (command.AccountType == AccountType.Crypto)
        {
            var cryptoAccount = CryptoAccount.Create(
                account.Id,
                command.CryptoSymbol!,
                command.CryptoNetwork,
                command.CryptoQuantity ?? 0m,
                command.CryptoAvgBuyPriceUsd);

            dbContext.CryptoAccounts.Add(cryptoAccount);
            await dbContext.SaveChangesAsync(cancellationToken);

            await dbContext.Entry(account).Reference(x => x.CryptoAccount).LoadAsync(cancellationToken);
        }

        if (command.AccountType == AccountType.CreditCard)
        {
            var creditCard = CreditCardAccount.Create(
                account.Id,
                command.CreditCardIssuer!,
                command.CreditCardBrand ?? CreditCardBrand.Other,
                command.CreditLimit ?? 500m,
                command.StatementClosingDay ?? 1,
                command.PaymentDueDay ?? 15,
                command.CreditCardProductName,
                command.CreditCardLastFour,
                command.OutstandingBalance ?? 0m,
                command.PaymentMode ?? CreditCardPaymentMode.Manual,
                command.RewardsProgram,
                command.StatementDelivery ?? CreditCardStatementDelivery.Virtual,
                command.InterestNominalAnnual,
                command.InterestEffectiveAnnual);

            dbContext.CreditCardAccounts.Add(creditCard);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Entry(account).Reference(x => x.CreditCardAccount).LoadAsync(cancellationToken);
        }

        return CreditCardMapping.ToAccountSummary(account);
    }
}
