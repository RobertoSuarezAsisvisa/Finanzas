namespace FinanzasMCP.Application.Common.DTOs;

public sealed record TransactionAttachmentSummary(
    Guid Id,
    Guid TransactionId,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTimeOffset UploadedAt,
    string ContentUrl);
