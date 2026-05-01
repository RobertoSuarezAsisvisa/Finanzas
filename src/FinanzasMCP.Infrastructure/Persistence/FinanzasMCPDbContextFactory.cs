using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FinanzasMCP.Infrastructure.Persistence;

public sealed class FinanzasMCPDbContextFactory : IDesignTimeDbContextFactory<FinanzasMCPDbContext>
{
    public FinanzasMCPDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<FinanzasMCPDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration["ConnectionStrings:Neon"]
            ?? configuration["DATABASE_URL"]
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing Neon connection string. Set ConnectionStrings:Neon, DATABASE_URL, or ConnectionStrings:DefaultConnection.");

        var options = new DbContextOptionsBuilder<FinanzasMCPDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new FinanzasMCPDbContext(options);
    }
}
