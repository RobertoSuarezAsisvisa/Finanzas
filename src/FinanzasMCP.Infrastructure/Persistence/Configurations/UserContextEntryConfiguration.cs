using FinanzasMCP.Domain.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanzasMCP.Infrastructure.Persistence.Configurations;

public sealed class UserContextEntryConfiguration : IEntityTypeConfiguration<UserContextEntry>
{
    public void Configure(EntityTypeBuilder<UserContextEntry> builder)
    {
        builder.ToTable("user_context");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Key).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Value).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.DeletedAt);
        builder.HasIndex(x => x.Key).IsUnique();
    }
}
