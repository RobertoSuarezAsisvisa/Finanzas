using FinanzasMCP.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class UserExternalLoginConfiguration : IEntityTypeConfiguration<UserExternalLogin>
{
    public void Configure(EntityTypeBuilder<UserExternalLogin> builder)
    {
        builder.ToTable("user_external_logins");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Provider).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ProviderUserId).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(320);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.LastLoginAt).IsRequired();
        builder.HasIndex(x => new { x.Provider, x.ProviderUserId }).IsUnique();
        builder.HasIndex(x => x.UserId);
    }
}
