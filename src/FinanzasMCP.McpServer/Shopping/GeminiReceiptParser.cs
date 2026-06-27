using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using FinanzasMCP.Application.Shopping;
using FinanzasMCP.Domain.Shopping;

namespace FinanzasMCP.McpServer.Shopping;

public sealed class GeminiReceiptParser(HttpClient httpClient, IConfiguration configuration) : IReceiptParser
{
    public async Task<ParsedReceipt> ParseAsync(Stream image, string contentType, CancellationToken cancellationToken)
    {
        var apiKey = configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Gemini:ApiKey is required to analyze receipt images.");
        }

        using var memory = new MemoryStream();
        await image.CopyToAsync(memory, cancellationToken);

        var model = configuration["Gemini:Model"];
        if (string.IsNullOrWhiteSpace(model))
        {
            model = "gemini-3.1-pro-preview";
        }

        var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(model)}:generateContent?key={Uri.EscapeDataString(apiKey)}";
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new StringContent(JsonSerializer.Serialize(BuildRequest(Convert.ToBase64String(memory.ToArray()), contentType)), Encoding.UTF8, "application/json")
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Gemini receipt analysis failed: {body}");
        }

        var outputText = ExtractOutputText(body);
        if (string.IsNullOrWhiteSpace(outputText))
        {
            throw new InvalidOperationException("Gemini receipt analysis did not return structured output.");
        }

        var parsed = JsonSerializer.Deserialize<ReceiptParserResponse>(outputText, new JsonSerializerOptions(JsonSerializerDefaults.Web))
            ?? throw new InvalidOperationException("Gemini receipt analysis returned invalid JSON.");

        return new ParsedReceipt(
            parsed.StoreName,
            DateTimeOffset.TryParse(parsed.ReceiptDate, out var receiptDate) ? receiptDate : null,
            parsed.Lines.Select(line => new ParsedReceiptLine(
                line.ProductName,
                line.VariantName,
                line.Quantity,
                Enum.TryParse<ShoppingUnit>(line.Unit, ignoreCase: true, out var unit) ? unit : ShoppingUnit.Unit,
                line.TotalPrice)).ToArray());
    }

    private static object BuildRequest(string base64Image, string contentType)
        => new
        {
            contents = new object[]
            {
                new
                {
                    role = "user",
                    parts = new object[]
                    {
                        new
                        {
                            text = "Read this grocery receipt image. Extract storeName, receiptDate, and line items. Use units only from: Unit, Gram, Kilogram, Milliliter, Liter, Pack. Return JSON only."
                        },
                        new
                        {
                            inlineData = new
                            {
                                mimeType = contentType,
                                data = base64Image
                            }
                        }
                    }
                }
            },
            generationConfig = new
            {
                responseMimeType = "application/json",
                responseSchema = ReceiptSchema
            }
        };

    private static string ExtractOutputText(string body)
    {
        var root = JsonNode.Parse(body);
        var candidates = root?["candidates"]?.AsArray();
        if (candidates is null || candidates.Count == 0)
        {
            return string.Empty;
        }

        var parts = candidates[0]?["content"]?["parts"]?.AsArray();
        if (parts is null)
        {
            return string.Empty;
        }

        foreach (var part in parts)
        {
            var text = part?["text"]?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
        }

        return string.Empty;
    }

    private static readonly object ReceiptSchema = new
    {
        type = "OBJECT",
        properties = new
        {
            storeName = new { type = "STRING", nullable = true },
            receiptDate = new { type = "STRING", nullable = true },
            lines = new
            {
                type = "ARRAY",
                items = new
                {
                    type = "OBJECT",
                    properties = new
                    {
                        productName = new { type = "STRING" },
                        variantName = new { type = "STRING" },
                        quantity = new { type = "NUMBER" },
                        unit = new { type = "STRING", format = "enum", @enum = new[] { "Unit", "Gram", "Kilogram", "Milliliter", "Liter", "Pack" } },
                        totalPrice = new { type = "NUMBER" }
                    },
                    required = new[] { "productName", "variantName", "quantity", "unit", "totalPrice" }
                }
            }
        },
        required = new[] { "storeName", "receiptDate", "lines" }
    };

    private sealed record ReceiptParserResponse(string? StoreName, string? ReceiptDate, IReadOnlyList<ReceiptParserLine> Lines);
    private sealed record ReceiptParserLine(string ProductName, string VariantName, decimal Quantity, string Unit, decimal TotalPrice);
}
