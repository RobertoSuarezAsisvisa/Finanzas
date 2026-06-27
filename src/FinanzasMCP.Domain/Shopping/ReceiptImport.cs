using FinanzasMCP.Domain.Common;

namespace FinanzasMCP.Domain.Shopping;

public sealed class ReceiptImport : UserOwnedEntity
{
    public Guid? StoreId { get; private set; }
    public string? DetectedStoreName { get; private set; }
    public DateTimeOffset? ReceiptDate { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public ReceiptImportStatus Status { get; private set; } = ReceiptImportStatus.PendingReview;
    public Store? Store { get; private set; }
    public ICollection<ReceiptImportLine> Lines { get; private set; } = new List<ReceiptImportLine>();

    public static ReceiptImport Create(string fileName, string contentType, long sizeBytes, string storagePath, string? detectedStoreName, DateTimeOffset? receiptDate)
        => new()
        {
            FileName = fileName.Trim(),
            ContentType = contentType.Trim(),
            SizeBytes = sizeBytes,
            StoragePath = storagePath.Trim(),
            DetectedStoreName = detectedStoreName?.Trim(),
            ReceiptDate = receiptDate
        };

    public void Confirm(Guid storeId)
    {
        StoreId = storeId;
        Status = ReceiptImportStatus.Confirmed;
        MarkUpdated();
    }
}
