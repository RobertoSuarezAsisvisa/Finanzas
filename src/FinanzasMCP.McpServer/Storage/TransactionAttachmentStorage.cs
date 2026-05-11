using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace FinanzasMCP.McpServer.Storage;

public interface ITransactionAttachmentStorage
{
    Task<string> UploadAsync(Guid userId, Guid transactionId, string fileName, string contentType, Stream content, CancellationToken cancellationToken);
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken);
    Task<(byte[] Content, string ContentType)> DownloadAsync(string storagePath, CancellationToken cancellationToken);
}

public sealed class TransactionAttachmentStorage(
    IConfiguration configuration,
    GoogleCredentialResolver credentialResolver) : ITransactionAttachmentStorage
{
    private readonly Lazy<StorageClient> _client = new(() =>
    {
        var credential = credentialResolver.Resolve()
            ?? throw new InvalidOperationException("Google Cloud credentials are not configured for Storage.");

        return StorageClient.Create(credential);
    });

    public async Task<string> UploadAsync(Guid userId, Guid transactionId, string fileName, string contentType, Stream content, CancellationToken cancellationToken)
    {
        var bucket = ResolveBucket();
        var objectName = $"users/{userId}/transactions/{transactionId}/{Guid.NewGuid():N}-{Sanitize(fileName)}";
        await _client.Value.UploadObjectAsync(bucket, objectName, contentType, content, cancellationToken: cancellationToken);
        return objectName;
    }

    public async Task DeleteAsync(string storagePath, CancellationToken cancellationToken)
    {
        await _client.Value.DeleteObjectAsync(ResolveBucket(), storagePath, cancellationToken: cancellationToken);
    }

    public async Task<(byte[] Content, string ContentType)> DownloadAsync(string storagePath, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        var storageObject = await _client.Value.GetObjectAsync(ResolveBucket(), storagePath, cancellationToken: cancellationToken);
        await _client.Value.DownloadObjectAsync(storageObject, stream, cancellationToken: cancellationToken);
        return (stream.ToArray(), storageObject.ContentType ?? "application/octet-stream");
    }

    private string ResolveBucket()
    {
        return configuration["Firebase:StorageBucket"]
            ?? throw new InvalidOperationException("Missing Firebase:StorageBucket configuration.");
    }

    private static string Sanitize(string fileName)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(fileName.Where(ch => !invalid.Contains(ch)).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(cleaned) ? "attachment" : cleaned.Replace(' ', '-');
    }
}
