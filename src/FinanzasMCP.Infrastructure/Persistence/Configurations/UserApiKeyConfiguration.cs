using FinanzasMCP.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class UserApiKeyConfiguration : IEntityTypeConfiguration<UserApiKey>
{
    public void Configure(EntityTypeBuilder<UserApiKey> builder)
    {
        builder.ToTable("user_api_keys");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.LookupKey).HasMaxLength(40).IsRequired();
        builder.Property(x => x.SecretHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.LastUsedAt);
        builder.Property(x => x.RevokedAt);
        builder.Property(x => x.DeletedAt);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.LookupKey).IsUnique();
        builder.HasOne(x => x.User).WithMany(x => x.ApiKeys).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
