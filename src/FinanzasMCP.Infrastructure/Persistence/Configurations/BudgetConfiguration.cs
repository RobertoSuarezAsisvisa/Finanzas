using FinanzasMCP.Domain.Budgets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("budgets");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LimitAmount).HasPrecision(18, 2);
        builder.Property(x => x.PeriodType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.ValidityType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.PeriodStart);
        builder.Property(x => x.PeriodEnd);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => new { x.Name, x.CategoryId, x.PeriodType, x.ValidityType, x.PeriodStart }).IsUnique();
    }
}
