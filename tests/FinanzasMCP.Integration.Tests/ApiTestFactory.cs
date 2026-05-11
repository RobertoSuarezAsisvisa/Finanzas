using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Infrastructure.Persistence;
using FinanzasMCP.McpServer.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;

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
            services.RemoveAll<ITransactionAttachmentStorage>();

            services.AddDbContext<FinanzasMCPDbContext>(options => options.UseSqlite(_connection));
            services.AddScoped<IFinanzasMCPDbContext>(sp => sp.GetRequiredService<FinanzasMCPDbContext>());
            services.AddSingleton<ITransactionAttachmentStorage, FakeTransactionAttachmentStorage>();
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

    private sealed class FakeTransactionAttachmentStorage : ITransactionAttachmentStorage
    {
        private readonly ConcurrentDictionary<string, (byte[] Content, string ContentType)> _files = new();

        public Task<string> UploadAsync(Guid userId, Guid transactionId, string fileName, string contentType, Stream content, CancellationToken cancellationToken)
        {
            using var memory = new MemoryStream();
            content.CopyTo(memory);
            var path = $"users/{userId}/transactions/{transactionId}/{fileName}";
            _files[path] = (memory.ToArray(), contentType);
            return Task.FromResult(path);
        }

        public Task DeleteAsync(string storagePath, CancellationToken cancellationToken)
        {
            _files.TryRemove(storagePath, out _);
            return Task.CompletedTask;
        }

        public Task<(byte[] Content, string ContentType)> DownloadAsync(string storagePath, CancellationToken cancellationToken)
        {
            return Task.FromResult(_files[storagePath]);
        }
    }
}
