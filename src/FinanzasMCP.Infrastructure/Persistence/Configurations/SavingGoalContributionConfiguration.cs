using FinanzasMCP.Domain.Contributions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class SavingGoalContributionConfiguration : IEntityTypeConfiguration<SavingGoalContribution>
{
    public void Configure(EntityTypeBuilder<SavingGoalContribution> builder)
    {
        builder.ToTable("saving_goal_contributions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.ContributionDate).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.DeletedAt);
        builder.HasIndex(x => x.GoalId);

        builder.HasOne(x => x.SavingGoal)
            .WithMany(g => g.Contributions)
            .HasForeignKey(x => x.GoalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SavingGoal)
            .WithMany(g => g.Contributions)
            .HasForeignKey(x => x.GoalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
