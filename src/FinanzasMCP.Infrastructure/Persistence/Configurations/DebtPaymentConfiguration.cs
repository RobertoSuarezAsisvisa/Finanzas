using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class DebtPaymentConfiguration : IEntityTypeConfiguration<DebtPayment>
{
    public void Configure(EntityTypeBuilder<DebtPayment> builder)
    {
        builder.ToTable("debt_payments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.PaymentDate).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.DeletedAt);
        builder.HasIndex(x => x.DebtId);

        builder.HasOne(x => x.Debt)
            .WithMany(d => d.Payments)
            .HasForeignKey(x => x.DebtId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
