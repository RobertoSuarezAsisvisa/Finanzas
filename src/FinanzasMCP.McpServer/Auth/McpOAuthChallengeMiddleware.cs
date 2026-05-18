namespace FinanzasMCP.McpServer.Auth;

public sealed class McpOAuthChallengeMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode != StatusCodes.Status401Unauthorized ||
            !context.Request.Path.StartsWithSegments("/mcp", StringComparison.OrdinalIgnoreCase) ||
            context.Response.HasStarted)
        {
            return;
        }

        var metadataUrl = OAuthMetadataEndpoints.GetProtectedResourceMetadataUrl(context.Request);
        context.Response.Headers.WWWAuthenticate =
            $"Bearer resource_metadata=\"{metadataUrl}\", scope=\"openid profile email finance:read finance:write\"";
    }
}
