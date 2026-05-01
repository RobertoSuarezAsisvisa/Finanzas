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
        var account = await dbContext.Accounts.FirstAsync(x => x.Id == command.Id, cancellationToken);
        account.UpdateDetails(command.Name, command.Currency, command.BankName, command.AccountNumber, command.Provider);

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
        return new AccountSummary(account.Id, account.Name, account.AccountType, account.Currency, account.Balance, account.IsActive, account.BankName, account.Provider, account.CryptoAccount?.Symbol, account.CryptoAccount?.Network, account.CryptoAccount?.Quantity, account.CryptoAccount?.AvgBuyPriceUsd);
    }
}
