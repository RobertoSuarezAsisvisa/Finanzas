using FinanzasMCP.Domain.Contributions;
using FinanzasMCP.Domain.Goals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class FinancialGoalConfiguration : IEntityTypeConfiguration<FinancialGoal>
{
    public void Configure(EntityTypeBuilder<FinancialGoal> builder)
    {
        builder.ToTable("financial_goals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.TargetAmount).HasPrecision(18, 2);
        builder.Property(x => x.CurrentAmount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Priority).IsRequired();
        builder.Property(x => x.Url).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.DeletedAt);
        builder.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => x.AccountId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.Type, x.Status, x.Priority });
    }
}

public sealed class FinancialGoalContributionConfiguration : IEntityTypeConfiguration<FinancialGoalContribution>
{
    public void Configure(EntityTypeBuilder<FinancialGoalContribution> builder)
    {
        builder.ToTable("financial_goal_contributions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.ContributionDate).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.DeletedAt);
        builder.HasOne(x => x.Goal).WithMany(x => x.Contributions).HasForeignKey(x => x.GoalId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Transaction).WithMany(x => x.FinancialGoalContributions).HasForeignKey(x => x.TransactionId).OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => x.GoalId);
        builder.HasIndex(x => x.TransactionId);
    }
}
