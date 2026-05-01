using FinanzasMCP.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(300).IsRequired();
        builder.Property(x => x.AccountType).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.Currency).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Balance).HasPrecision(18, 2);
        builder.Property(x => x.BankName).HasMaxLength(200);
        builder.Property(x => x.AccountNumber).HasMaxLength(100);
        builder.Property(x => x.Provider).HasMaxLength(100);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => x.Name);
        builder.HasOne(x => x.CryptoAccount).WithOne(x => x.Account).HasForeignKey<CryptoAccount>(x => x.AccountId);
    }
}
