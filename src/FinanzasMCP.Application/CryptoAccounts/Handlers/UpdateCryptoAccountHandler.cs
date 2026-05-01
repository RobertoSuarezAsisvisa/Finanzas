using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.CryptoAccounts.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.CryptoAccounts.Handlers;

public sealed class UpdateCryptoAccountHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<CryptoAccountSummary> Handle(UpdateCryptoAccountCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Symbol))
        {
            throw new InvalidOperationException("Symbol is required.");
        }

        if (command.Quantity < 0)
        {
            throw new InvalidOperationException("Quantity cannot be negative.");
        }

        var account = await dbContext.Accounts.FirstAsync(x => x.Id == command.AccountId, cancellationToken);
        if (account.AccountType != AccountType.Crypto)
        {
            throw new InvalidOperationException("Crypto account details can only be assigned to crypto accounts.");
        }

        var cryptoAccount = await dbContext.CryptoAccounts.FirstAsync(x => x.Id == command.Id, cancellationToken);
        cryptoAccount.UpdateDetails(command.AccountId, command.Symbol, command.Network, command.Quantity, command.AvgBuyPriceUsd);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CryptoAccountSummary(
            cryptoAccount.Id,
            cryptoAccount.AccountId,
            cryptoAccount.Symbol,
            cryptoAccount.Network,
            cryptoAccount.Quantity,
            cryptoAccount.AvgBuyPriceUsd);
    }
}
