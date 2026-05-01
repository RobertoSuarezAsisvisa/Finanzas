using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Accounts.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
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

        var account = Account.Create(
            command.Name,
            command.AccountType,
            command.Currency,
            command.Balance,
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

        return Map(account);
    }

    private static AccountSummary Map(Account account)
    {
        return new AccountSummary(
            account.Id,
            account.Name,
            account.AccountType,
            account.Currency,
            account.Balance,
            account.IsActive,
            account.BankName,
            account.Provider,
            account.CryptoAccount?.Symbol,
            account.CryptoAccount?.Network,
            account.CryptoAccount?.Quantity,
            account.CryptoAccount?.AvgBuyPriceUsd);
    }
}
