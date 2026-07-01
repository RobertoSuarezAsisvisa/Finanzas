using FinanzasMCP.Domain.CreditCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class CreditCardTransactionConfiguration : IEntityTypeConfiguration<CreditCardTransaction>
{
    public void Configure(EntityTypeBuilder<CreditCardTransaction> builder)
    {
        builder.ToTable("credit_card_transactions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OperationType).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.Merchant).HasMaxLength(200);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasOne(x => x.Transaction).WithOne().HasForeignKey<CreditCardTransaction>(x => x.TransactionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CreditCardAccount).WithMany(x => x.CreditCardTransactions).HasForeignKey(x => x.CreditCardAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Statement).WithMany().HasForeignKey(x => x.StatementId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => x.TransactionId).IsUnique();
        builder.HasIndex(x => x.CreditCardAccountId);
        builder.HasIndex(x => x.StatementId);
        builder.HasIndex(x => x.OperationType);
    }
}
