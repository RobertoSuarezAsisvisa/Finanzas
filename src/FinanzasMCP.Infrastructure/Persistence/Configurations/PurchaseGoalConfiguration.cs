using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class PurchaseGoalConfiguration : IEntityTypeConfiguration<PurchaseGoal>
{
    public void Configure(EntityTypeBuilder<PurchaseGoal> builder)
    {
        builder.ToTable("purchase_goals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.GoalPrice).HasPrecision(18, 2);
        builder.Property(x => x.SavedAmount).HasPrecision(18, 2);
        builder.Property(x => x.Priority).IsRequired();
        builder.Property(x => x.Url).HasMaxLength(500);
        builder.Property(x => x.TargetDate);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => new { x.Status, x.Priority });
    }
}
