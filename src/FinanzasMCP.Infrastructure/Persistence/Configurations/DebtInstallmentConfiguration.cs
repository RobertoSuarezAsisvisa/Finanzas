using FinanzasMCP.Domain.Debts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class DebtInstallmentConfiguration : IEntityTypeConfiguration<DebtInstallment>
{
    public void Configure(EntityTypeBuilder<DebtInstallment> builder)
    {
        builder.ToTable("debt_installments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DueDate).IsRequired();
        builder.Property(x => x.ExpectedPayment).HasPrecision(18, 2);
        builder.Property(x => x.Principal).HasPrecision(18, 2);
        builder.Property(x => x.Interest).HasPrecision(18, 2);
        builder.Property(x => x.BalanceAfterPayment).HasPrecision(18, 2);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.DeletedAt);
        builder.HasIndex(x => new { x.DebtId, x.Number }).IsUnique();
        builder.HasIndex(x => x.DueDate);

        builder.HasOne(x => x.Debt)
            .WithMany(d => d.Installments)
            .HasForeignKey(x => x.DebtId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
