using FinanzasMCP.Domain.Recurring;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class RecurringRuleConfiguration : IEntityTypeConfiguration<RecurringRule>
{
    public void Configure(EntityTypeBuilder<RecurringRule> builder)
    {
        builder.ToTable("recurring_rules");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Frequency).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.NextDueDate).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => new { x.AccountId, x.IsActive });
    }
}
