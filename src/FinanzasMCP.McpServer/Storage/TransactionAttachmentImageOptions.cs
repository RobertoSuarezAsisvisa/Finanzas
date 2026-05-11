namespace FinanzasMCP.McpServer.Storage;

public sealed class TransactionAttachmentImageOptions
{
    public const string SectionName = "Attachments:ImageProcessing";

    public int MaxDimension { get; set; } = 1600;
    public int JpegQuality { get; set; } = 76;
}
