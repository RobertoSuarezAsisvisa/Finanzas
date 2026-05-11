using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FinanzasMCP.McpServer.Storage;

public interface ITransactionAttachmentProcessor
{
    Task<PreparedTransactionAttachment> PrepareAsync(IFormFile file, CancellationToken cancellationToken);
}

public sealed record PreparedTransactionAttachment(
    string FileName,
    string ContentType,
    long SizeBytes,
    Stream Content) : IAsyncDisposable
{
    public ValueTask DisposeAsync() => Content.DisposeAsync();
}

public sealed class TransactionAttachmentProcessor(
    IOptions<TransactionAttachmentImageOptions> options,
    ILogger<TransactionAttachmentProcessor> logger) : ITransactionAttachmentProcessor
{
    private readonly TransactionAttachmentImageOptions _options = options.Value;

    public async Task<PreparedTransactionAttachment> PrepareAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var source = file.OpenReadStream();
        var original = await CopyToMemoryAsync(source, cancellationToken);

        if (!IsImage(file.ContentType))
        {
            return CreatePrepared(file.FileName, file.ContentType, original);
        }

        try
        {
            await using var originalForDecode = CreateReadableClone(original);
            using var image = await Image.LoadAsync(originalForDecode, cancellationToken);
            image.Mutate(context =>
            {
                context.AutoOrient();

                if (image.Width > _options.MaxDimension || image.Height > _options.MaxDimension)
                {
                    context.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(_options.MaxDimension, _options.MaxDimension)
                    });
                }
            });

            var hasTransparency = HasTransparency(image);
            var targetContentType = hasTransparency ? "image/png" : "image/jpeg";
            var targetFileName = BuildTargetFileName(file.FileName, hasTransparency ? ".png" : ".jpg");

            var processed = new MemoryStream();
            if (hasTransparency)
            {
                await image.SaveAsPngAsync(processed, new PngEncoder(), cancellationToken);
            }
            else
            {
                await image.SaveAsJpegAsync(processed, new JpegEncoder { Quality = _options.JpegQuality }, cancellationToken);
            }

            if (processed.Length >= original.Length)
            {
                await processed.DisposeAsync();
                return CreatePrepared(file.FileName, file.ContentType, original);
            }

            await original.DisposeAsync();
            processed.Position = 0;
            return new PreparedTransactionAttachment(targetFileName, targetContentType, processed.Length, processed);
        }
        catch (UnknownImageFormatException ex)
        {
            logger.LogDebug(ex, "Skipping optimization for unsupported image format {ContentType}", file.ContentType);
            return CreatePrepared(file.FileName, file.ContentType, original);
        }
        catch (InvalidImageContentException ex)
        {
            logger.LogDebug(ex, "Skipping optimization for invalid image payload {FileName}", file.FileName);
            return CreatePrepared(file.FileName, file.ContentType, original);
        }
    }

    private static bool IsImage(string contentType)
        => contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);

    private static PreparedTransactionAttachment CreatePrepared(string fileName, string contentType, MemoryStream content)
    {
        content.Position = 0;
        return new PreparedTransactionAttachment(fileName, contentType, content.Length, content);
    }

    private static async Task<MemoryStream> CopyToMemoryAsync(Stream source, CancellationToken cancellationToken)
    {
        var memory = new MemoryStream();
        await source.CopyToAsync(memory, cancellationToken);
        memory.Position = 0;
        return memory;
    }

    private static MemoryStream CreateReadableClone(MemoryStream source)
    {
        return new MemoryStream(source.ToArray(), writable: false);
    }

    private static string BuildTargetFileName(string fileName, string extension)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "attachment";
        }

        return $"{name.Trim()}{extension}";
    }

    private static bool HasTransparency(Image image)
    {
        using var rgbaImage = image.CloneAs<Rgba32>();
        var hasTransparency = false;

        rgbaImage.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height && !hasTransparency; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (var x = 0; x < row.Length; x++)
                {
                    if (row[x].A < byte.MaxValue)
                    {
                        hasTransparency = true;
                        break;
                    }
                }
            }
        });

        return hasTransparency;
    }
}
