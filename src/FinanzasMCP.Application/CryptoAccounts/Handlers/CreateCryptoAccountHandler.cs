using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.CryptoAccounts.Commands;
using FinanzasMCP.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.CryptoAccounts.Handlers;

public sealed class CreateCryptoAccountHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<CryptoAccountSummary> Handle(CreateCryptoAccountCommand command, CancellationToken cancellationToken = default)
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

        if (await dbContext.CryptoAccounts.AnyAsync(x => x.AccountId == command.AccountId, cancellationToken))
        {
            throw new InvalidOperationException("This account already has crypto account details.");
        }

        var cryptoAccount = CryptoAccount.Create(
            command.AccountId,
            command.Symbol,
            command.Network,
            command.Quantity,
            command.AvgBuyPriceUsd);

        dbContext.CryptoAccounts.Add(cryptoAccount);
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
