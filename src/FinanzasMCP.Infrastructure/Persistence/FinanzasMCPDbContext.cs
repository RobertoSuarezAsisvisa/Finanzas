using FinanzasMCP.Domain.AccountingPeriods;
using FinanzasMCP.Domain.Accounts;
using FinanzasMCP.Domain.Budgets;
using FinanzasMCP.Domain.Categories;
using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Crypto;
using FinanzasMCP.Domain.Debts;
using FinanzasMCP.Domain.Goals;
using FinanzasMCP.Domain.Recurring;
using FinanzasMCP.Domain.Tags;
using FinanzasMCP.Domain.Transactions;
using FinanzasMCP.Domain.UserContext;
using FinanzasMCP.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using FinanzasMCP.Domain.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace FinanzasMCP.Infrastructure.Persistence;

public sealed class FinanzasMCPDbContext(DbContextOptions<FinanzasMCPDbContext> options) : DbContext(options), IFinanzasMCPDbContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<CryptoAccount> CryptoAccounts => Set<CryptoAccount>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionTag> TransactionTags => Set<TransactionTag>();
    public DbSet<CryptoLot> CryptoLots => Set<CryptoLot>();
    public DbSet<RecurringRule> RecurringRules => Set<RecurringRule>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<SavingGoal> SavingGoals => Set<SavingGoal>();
    public DbSet<SavingGoalContribution> SavingGoalContributions => Set<SavingGoalContribution>();
    public DbSet<PurchaseGoal> PurchaseGoals => Set<PurchaseGoal>();
    public DbSet<PurchaseGoalContribution> PurchaseGoalContributions => Set<PurchaseGoalContribution>();
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<DebtPayment> DebtPayments => Set<DebtPayment>();
    public DbSet<AccountingPeriod> AccountingPeriods => Set<AccountingPeriod>();
    public DbSet<UserContextEntry> UserContextEntries => Set<UserContextEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanzasMCPDbContext).Assembly);
        ApplySoftDeleteFilters(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(SoftDeletableEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(FinanzasMCPDbContext)
                    .GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, new object[] { modelBuilder });
            }
        }
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : SoftDeletableEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(x => x.DeletedAt == null);
    }
}
