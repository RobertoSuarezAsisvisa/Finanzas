using FinanzasMCP.Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class TransactionTagConfiguration : IEntityTypeConfiguration<TransactionTag>
{
    public void Configure(EntityTypeBuilder<TransactionTag> builder)
    {
        builder.ToTable("transaction_tags");
        builder.HasKey(x => new { x.TransactionId, x.TagId });
        builder.HasOne(x => x.Tag)
            .WithMany(t => t.TransactionTags)
            .HasForeignKey(x => x.TagId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
