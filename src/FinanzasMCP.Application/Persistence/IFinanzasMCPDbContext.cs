using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.AccountingPeriods;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Crypto;
using FinanzasMCP.Domain.CreditCards;
using FinanzasMCP.Domain.Goals;
using FinanzasMCP.Domain.Shopping;
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
    DbSet<CreditCardAccount> CreditCardAccounts { get; }
    DbSet<CreditCardStatement> CreditCardStatements { get; }
    DbSet<CreditCardTransaction> CreditCardTransactions { get; }
    DbSet<CryptoLot> CryptoLots { get; }
    DbSet<AccountingPeriod> AccountingPeriods { get; }
    DbSet<TransactionAttachment> TransactionAttachments { get; }
    DbSet<FinancialGoal> FinancialGoals { get; }
    DbSet<FinancialGoalContribution> FinancialGoalContributions { get; }
    DbSet<Store> Stores { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<StoreProductPrice> StoreProductPrices { get; }
    DbSet<ShoppingList> ShoppingLists { get; }
    DbSet<ShoppingListItem> ShoppingListItems { get; }
    DbSet<ReceiptImport> ReceiptImports { get; }
    DbSet<ReceiptImportLine> ReceiptImportLines { get; }
    DbSet<AppUser> Users { get; }
    DbSet<UserApiKey> UserApiKeys { get; }
    DbSet<UserExternalLogin> UserExternalLogins { get; }

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
