using FinanzasMCP.Application.Accounts.Commands;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
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

        var account = await dbContext.Accounts
            .Include(x => x.CryptoAccount)
            .FirstAsync(x => x.Id == command.Id, cancellationToken);

        account.UpdateDetails(command.Name, command.AccountType, command.Currency, command.Balance, command.BankName, command.AccountNumber, command.Provider);

        await SyncCryptoAccount(command, cancellationToken);

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
        return Map(account);
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

    private static AccountSummary Map(Account account)
    {
        var cryptoAccount = account.AccountType == AccountType.Crypto ? account.CryptoAccount : null;

        return new AccountSummary(
            account.Id,
            account.Name,
            account.AccountType,
            account.Currency,
            account.Balance,
            account.IsActive,
            account.BankName,
            account.AccountNumber,
            account.Provider,
            cryptoAccount?.Symbol,
            cryptoAccount?.Network,
            cryptoAccount?.Quantity,
            cryptoAccount?.AvgBuyPriceUsd);
    }
}
