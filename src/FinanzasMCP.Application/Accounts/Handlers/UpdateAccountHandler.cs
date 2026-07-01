using FinanzasMCP.Application.Accounts.Commands;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.CreditCards.Handlers;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.CreditCards;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Accounts.Handlers;

public sealed class UpdateAccountHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<AccountSummary> Handle(UpdateAccountCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new InvalidOperationException("Account name is required.");
        }

        if (command.Balance < 0)
        {
            throw new InvalidOperationException("Balance cannot be negative.");
        }

        if (command.AccountType == AccountType.Crypto && string.IsNullOrWhiteSpace(command.CryptoSymbol))
        {
            throw new InvalidOperationException("Crypto symbol is required for crypto accounts.");
        }

        if (command.AccountType == AccountType.CreditCard && string.IsNullOrWhiteSpace(command.CreditCardIssuer))
        {
            throw new InvalidOperationException("Issuer is required for credit card accounts.");
        }

        var account = await dbContext.Accounts
            .Include(x => x.CryptoAccount)
            .Include(x => x.CreditCardAccount)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);

        account.UpdateDetails(command.Name, command.AccountType, command.Currency, command.Purpose, command.AccountType == AccountType.CreditCard ? 0m : command.Balance, command.BankName, command.AccountNumber, command.Provider);

        await SyncCryptoAccount(command, cancellationToken);
        await SyncCreditCardAccount(command, cancellationToken);

        if (command.IsActive)
        {
            account.Activate();
        }
        else
        {
            account.Deactivate();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(account).Reference(x => x.CryptoAccount).LoadAsync(cancellationToken);
        await dbContext.Entry(account).Reference(x => x.CreditCardAccount).LoadAsync(cancellationToken);
        return CreditCardMapping.ToAccountSummary(account);
    }

    private async Task SyncCryptoAccount(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        var cryptoAccount = await dbContext.CryptoAccounts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.AccountId == command.Id, cancellationToken);

        if (command.AccountType != AccountType.Crypto)
        {
            cryptoAccount?.SoftDelete();
            return;
        }

        if (cryptoAccount is null)
        {
            dbContext.CryptoAccounts.Add(CryptoAccount.Create(
                command.Id,
                command.CryptoSymbol!,
                command.CryptoNetwork,
                command.CryptoQuantity ?? 0m,
                command.CryptoAvgBuyPriceUsd));
            return;
        }

        cryptoAccount.Restore();
        cryptoAccount.UpdateDetails(
            command.Id,
            command.CryptoSymbol!,
            command.CryptoNetwork,
            command.CryptoQuantity ?? 0m,
            command.CryptoAvgBuyPriceUsd);
    }

    private async Task SyncCreditCardAccount(UpdateAccountCommand command, CancellationToken cancellationToken)
    {
        var creditCard = await dbContext.CreditCardAccounts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.AccountId == command.Id, cancellationToken);

        if (command.AccountType != AccountType.CreditCard)
        {
            creditCard?.SoftDelete();
            return;
        }

        if (creditCard is null)
        {
            dbContext.CreditCardAccounts.Add(CreditCardAccount.Create(
                command.Id,
                command.CreditCardIssuer!,
                command.CreditCardBrand ?? CreditCardBrand.Other,
                command.CreditLimit ?? 500m,
                command.StatementClosingDay ?? 1,
                command.PaymentDueDay ?? 15,
                command.CreditCardProductName,
                command.CreditCardLastFour,
                0m,
                command.PaymentMode ?? CreditCardPaymentMode.Manual,
                command.RewardsProgram,
                command.StatementDelivery ?? CreditCardStatementDelivery.Virtual,
                command.InterestNominalAnnual,
                command.InterestEffectiveAnnual));
            return;
        }

        creditCard.Restore();
        creditCard.UpdateDetails(
            command.CreditCardIssuer!,
            command.CreditCardBrand ?? CreditCardBrand.Other,
            command.CreditLimit ?? creditCard.CreditLimit,
            command.StatementClosingDay ?? creditCard.StatementClosingDay,
            command.PaymentDueDay ?? creditCard.PaymentDueDay,
            command.CreditCardProductName,
            command.CreditCardLastFour,
            command.PaymentMode ?? creditCard.PaymentMode,
            command.RewardsProgram,
            command.StatementDelivery ?? creditCard.StatementDelivery,
            command.InterestNominalAnnual,
            command.InterestEffectiveAnnual,
            command.IsActive);
    }
}
