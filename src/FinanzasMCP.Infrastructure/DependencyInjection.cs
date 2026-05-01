using FinanzasMCP.Infrastructure.Persistence;
using FinanzasMCP.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanzasMCP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:Neon"]
            ?? configuration["DATABASE_URL"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Missing Neon connection string. Set ConnectionStrings:Neon, DATABASE_URL, or ConnectionStrings:DefaultConnection.");
        }

        services.AddDbContext<FinanzasMCPDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IFinanzasMCPDbContext>(sp => sp.GetRequiredService<FinanzasMCPDbContext>());
        return services;
    }
}
