using FinanzasMCP.Domain.Crypto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class CryptoLotConfiguration : IEntityTypeConfiguration<CryptoLot>
{
    public void Configure(EntityTypeBuilder<CryptoLot> builder)
    {
        builder.ToTable("crypto_lots");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasPrecision(18, 8);
        builder.Property(x => x.BuyPriceUsd).HasPrecision(18, 2);
        builder.Property(x => x.SellPriceUsd).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.OperationDate).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => x.AccountId);

        builder.HasOne(x => x.Account)
            .WithMany(a => a.Lots)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
