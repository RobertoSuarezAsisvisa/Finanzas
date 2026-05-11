using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.AccountingPeriods;
using FinanzasMCP.Domain.Crypto;
using FinanzasMCP.Domain.Transactions;
using FinanzasMCP.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FinanzasMCP.Application.Persistence;

public interface IFinanzasMCPDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    DbSet<Account> Accounts { get; }
    DbSet<CryptoAccount> CryptoAccounts { get; }
    DbSet<CryptoLot> CryptoLots { get; }
    DbSet<AccountingPeriod> AccountingPeriods { get; }
    DbSet<TransactionAttachment> TransactionAttachments { get; }
    DbSet<AppUser> Users { get; }
    DbSet<UserApiKey> UserApiKeys { get; }
    DbSet<UserExternalLogin> UserExternalLogins { get; }

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
