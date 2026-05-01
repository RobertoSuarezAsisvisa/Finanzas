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
            ?? configuration.GetConnectionString("Neon")
            ?? throw new InvalidOperationException("Missing Neon connection string. Set ConnectionStrings:Neon.");

        var options = new DbContextOptionsBuilder<FinanzasMCPDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new FinanzasMCPDbContext(options);
    }
}
