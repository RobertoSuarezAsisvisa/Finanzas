using FinanzasMCP.Domain.Contributions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class PurchaseGoalContributionConfiguration : IEntityTypeConfiguration<PurchaseGoalContribution>
{
    public void Configure(EntityTypeBuilder<PurchaseGoalContribution> builder)
    {
        builder.ToTable("purchase_goal_contributions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.ContributionDate).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.DeletedAt);
        builder.HasIndex(x => x.PurchaseGoalId);

        builder.HasOne(x => x.PurchaseGoal)
            .WithMany(x => x.Contributions)
            .HasForeignKey(x => x.PurchaseGoalId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(x => x.PurchaseGoalId);
    }
}
