using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Transactions;

public sealed class TransactionAttachment : UserOwnedEntity
{
    public Guid TransactionId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public DateTimeOffset UploadedAt { get; private set; } = DateTimeOffset.UtcNow;
    public Transaction Transaction { get; private set; } = null!;

    public static TransactionAttachment Create(
        Guid transactionId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath)
        => new()
        {
            TransactionId = transactionId,
            FileName = fileName.Trim(),
            ContentType = contentType.Trim(),
            SizeBytes = sizeBytes,
            StoragePath = storagePath.Trim(),
            UploadedAt = DateTimeOffset.UtcNow
        };
}
