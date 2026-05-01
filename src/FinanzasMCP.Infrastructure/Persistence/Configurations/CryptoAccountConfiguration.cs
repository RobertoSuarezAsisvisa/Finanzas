using FinanzasMCP.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class CryptoAccountConfiguration : IEntityTypeConfiguration<CryptoAccount>
{
    public void Configure(EntityTypeBuilder<CryptoAccount> builder)
    {
        builder.ToTable("crypto_accounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Symbol).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Network).HasMaxLength(50);
        builder.Property(x => x.Quantity).HasPrecision(18, 8);
        builder.Property(x => x.AvgBuyPriceUsd).HasPrecision(18, 2);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => x.AccountId).IsUnique();
    }
}
