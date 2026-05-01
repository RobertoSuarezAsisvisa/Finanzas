using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class SavingGoalConfiguration : IEntityTypeConfiguration<SavingGoal>
{
    public void Configure(EntityTypeBuilder<SavingGoal> builder)
    {
        builder.ToTable("saving_goals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.GoalAmount).HasPrecision(18, 2);
        builder.Property(x => x.CurrentAmount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.HasIndex(x => x.Status);
    }
}
