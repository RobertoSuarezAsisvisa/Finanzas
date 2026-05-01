using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FinanzasMCP.Integration.Tests;

public sealed class ApiTestFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public ApiTestFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Neon"] = "Data Source=:memory:"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<FinanzasMCPDbContext>>();
            services.RemoveAll<FinanzasMCPDbContext>();
            services.RemoveAll<IFinanzasMCPDbContext>();

            services.AddDbContext<FinanzasMCPDbContext>(options => options.UseSqlite(_connection));
            services.AddScoped<IFinanzasMCPDbContext>(sp => sp.GetRequiredService<FinanzasMCPDbContext>());
        });
    }

    public void InitializeDatabase()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinanzasMCPDbContext>();
        dbContext.Database.EnsureCreated();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
