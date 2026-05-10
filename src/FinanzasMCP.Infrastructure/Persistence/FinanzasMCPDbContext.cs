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
using FinanzasMCP.Domain.Users;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Auth;
using Microsoft.EntityFrameworkCore;
using FinanzasMCP.Domain.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Reflection;

namespace FinanzasMCP.Infrastructure.Persistence;

public sealed class FinanzasMCPDbContext(
    DbContextOptions<FinanzasMCPDbContext> options,
    ICurrentUser? currentUser = null) : DbContext(options), IFinanzasMCPDbContext
{
    public Guid? CurrentUserId => currentUser?.UserId;

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<UserExternalLogin> UserExternalLogins => Set<UserExternalLogin>();
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
    public DbSet<DebtInstallment> DebtInstallments => Set<DebtInstallment>();
    public DbSet<AccountingPeriod> AccountingPeriods => Set<AccountingPeriod>();
    public DbSet<UserContextEntry> UserContextEntries => Set<UserContextEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanzasMCPDbContext).Assembly);
        ApplyGlobalFilters(modelBuilder);
        ConfigureUserOwnedEntities(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AssignCurrentUserToNewEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AssignCurrentUserToNewEntities()
    {
        var entries = ChangeTracker.Entries<IUserOwnedEntity>()
            .Where(entry => entry.State == EntityState.Added)
            .ToArray();

        if (entries.Length == 0)
        {
            return;
        }

        var userId = CurrentUserId
            ?? throw new InvalidOperationException("An authenticated user is required to create financial records.");

        foreach (var entry in entries)
        {
            entry.Entity.AssignUser(userId);
        }
    }

    private static void ConfigureUserOwnedEntities(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(IUserOwnedEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid>(nameof(IUserOwnedEntity.UserId))
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .HasIndex(nameof(IUserOwnedEntity.UserId));
            }
        }
    }

    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(SoftDeletableEntity).IsAssignableFrom(entityType.ClrType) ||
                typeof(IUserOwnedEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(FinanzasMCPDbContext)
                    .GetMethod(nameof(SetGlobalFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private void SetGlobalFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        Expression? filter = null;

        if (typeof(SoftDeletableEntity).IsAssignableFrom(typeof(TEntity)))
        {
            var deletedAt = Expression.Property(parameter, nameof(SoftDeletableEntity.DeletedAt));
            filter = Expression.Equal(deletedAt, Expression.Constant(null, typeof(DateTimeOffset?)));
        }

        if (typeof(IUserOwnedEntity).IsAssignableFrom(typeof(TEntity)))
        {
            var userId = Expression.Property(parameter, nameof(IUserOwnedEntity.UserId));
            var currentUserId = Expression.Property(Expression.Constant(this), nameof(CurrentUserId));
            var userFilter = Expression.Equal(
                Expression.Convert(userId, typeof(Guid?)),
                currentUserId);

            filter = filter is null ? userFilter : Expression.AndAlso(filter, userFilter);
        }

        if (filter is not null)
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(Expression.Lambda<Func<TEntity, bool>>(filter, parameter));
        }
    }
}
