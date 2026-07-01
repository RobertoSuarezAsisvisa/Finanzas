using FinanzasMCP.Domain.CreditCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class CreditCardStatementConfiguration : IEntityTypeConfiguration<CreditCardStatement>
{
    public void Configure(EntityTypeBuilder<CreditCardStatement> builder)
    {
        builder.ToTable("credit_card_statements");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OpeningBalance).HasPrecision(18, 2);
        builder.Property(x => x.Purchases).HasPrecision(18, 2);
        builder.Property(x => x.Fees).HasPrecision(18, 2);
        builder.Property(x => x.Interest).HasPrecision(18, 2);
        builder.Property(x => x.Payments).HasPrecision(18, 2);
        builder.Property(x => x.StatementBalance).HasPrecision(18, 2);
        builder.Property(x => x.MinimumPayment).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasOne(x => x.CreditCardAccount).WithMany(x => x.Statements).HasForeignKey(x => x.CreditCardAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.CreditCardAccountId);
        builder.HasIndex(x => x.DueDate);
        builder.HasIndex(x => x.Status);
    }
}
