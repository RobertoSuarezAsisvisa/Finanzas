using FinanzasMCP.Domain.CreditCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class CreditCardAccountConfiguration : IEntityTypeConfiguration<CreditCardAccount>
{
    public void Configure(EntityTypeBuilder<CreditCardAccount> builder)
    {
        builder.ToTable("credit_card_accounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Issuer).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Brand).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.ProductName).HasMaxLength(200);
        builder.Property(x => x.LastFour).HasMaxLength(4);
        builder.Property(x => x.CreditLimit).HasPrecision(18, 2);
        builder.Property(x => x.OutstandingBalance).HasPrecision(18, 2);
        builder.Property(x => x.AvailableCredit).HasPrecision(18, 2);
        builder.Property(x => x.PaymentMode).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.RewardsProgram).HasMaxLength(100);
        builder.Property(x => x.StatementDelivery).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.InterestNominalAnnual).HasPrecision(9, 4);
        builder.Property(x => x.InterestEffectiveAnnual).HasPrecision(9, 4);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => x.AccountId).IsUnique();
        builder.HasIndex(x => x.IsActive);
    }
}
