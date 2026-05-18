using System.Security.Claims;
using System.Text.Json;

namespace FinanzasMCP.McpServer.Auth;

public sealed class McpOAuthScopeMiddleware(RequestDelegate next)
{
    private const string ReadScope = "finance:read";
    private const string WriteScope = "finance:write";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!IsMcpRequest(context) || !IsOAuthUser(context.User))
        {
            await next(context);
            return;
        }

        if (!HasScope(context.User, ReadScope))
        {
            await WriteForbiddenAsync(context, $"Missing required OAuth scope '{ReadScope}'.");
            return;
        }

        if (HttpMethods.IsPost(context.Request.Method) &&
            await CallsWriteToolAsync(context.Request, context.RequestAborted) &&
            !HasScope(context.User, WriteScope))
        {
            await WriteForbiddenAsync(context, $"Missing required OAuth scope '{WriteScope}'.");
            return;
        }

        await next(context);
    }

    private static bool IsMcpRequest(HttpContext context)
        => context.Request.Path.StartsWithSegments("/mcp", StringComparison.OrdinalIgnoreCase);

    private static bool IsOAuthUser(ClaimsPrincipal principal)
        => principal.HasClaim("auth_method", "oauth");

    private static bool HasScope(ClaimsPrincipal principal, string requiredScope)
    {
        var scopeClaims = principal.FindAll("scope")
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        var permissionClaims = principal.FindAll("permissions").Select(claim => claim.Value);

        return scopeClaims.Concat(permissionClaims)
            .Any(scope => string.Equals(scope, requiredScope, StringComparison.Ordinal));
    }

    private static async Task<bool> CallsWriteToolAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        if (request.ContentLength is 0 || request.Body is null || !request.Body.CanRead)
        {
            return false;
        }

        request.EnableBuffering();
        try
        {
            using var document = await JsonDocument.ParseAsync(request.Body, cancellationToken: cancellationToken);
            return JsonElementCallsWriteTool(document.RootElement);
        }
        catch (JsonException)
        {
            return false;
        }
        finally
        {
            request.Body.Position = 0;
        }
    }

    private static bool JsonElementCallsWriteTool(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                if (JsonElementCallsWriteTool(item))
                {
                    return true;
                }
            }

            return false;
        }

        if (element.ValueKind != JsonValueKind.Object ||
            !element.TryGetProperty("method", out var method) ||
            method.ValueKind != JsonValueKind.String ||
            !string.Equals(method.GetString(), "tools/call", StringComparison.Ordinal))
        {
            return false;
        }

        if (!element.TryGetProperty("params", out var parameters) ||
            parameters.ValueKind != JsonValueKind.Object ||
            !parameters.TryGetProperty("name", out var name) ||
            name.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        return IsWriteToolName(name.GetString());
    }

    private static bool IsWriteToolName(string? toolName)
        => !string.IsNullOrWhiteSpace(toolName) &&
            (toolName.StartsWith("Create", StringComparison.Ordinal) ||
             toolName.StartsWith("Update", StringComparison.Ordinal) ||
             toolName.StartsWith("Delete", StringComparison.Ordinal) ||
             toolName.StartsWith("Register", StringComparison.Ordinal) ||
             toolName.StartsWith("Add", StringComparison.Ordinal) ||
             toolName.StartsWith("Upsert", StringComparison.Ordinal));

    private static async Task WriteForbiddenAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message });
    }
}
