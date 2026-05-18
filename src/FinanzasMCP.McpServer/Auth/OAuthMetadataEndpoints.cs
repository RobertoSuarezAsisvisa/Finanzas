namespace FinanzasMCP.McpServer.Auth;

public static class OAuthMetadataEndpoints
{
    private static readonly string[] Scopes =
    [
        "openid",
        "profile",
        "email",
        "finance:read",
        "finance:write"
    ];

    public static IEndpointRouteBuilder MapOAuthMetadataEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/.well-known/oauth-protected-resource", CreateMetadata).AllowAnonymous();
        app.MapGet("/.well-known/oauth-protected-resource/mcp", CreateMetadata).AllowAnonymous();
        return app;
    }

    public static string GetProtectedResourceMetadataUrl(HttpRequest request)
        => $"{request.Scheme}://{request.Host}/.well-known/oauth-protected-resource/mcp";

    private static IResult CreateMetadata(HttpContext context, IConfiguration configuration)
    {
        var authority = NormalizeAuthority(configuration["OAuth:Authority"]);
        if (string.IsNullOrWhiteSpace(authority))
        {
            return Results.NotFound(new { message = "OAuth authority is not configured." });
        }

        var resource = configuration["OAuth:Resource"];
        if (string.IsNullOrWhiteSpace(resource))
        {
            resource = $"{context.Request.Scheme}://{context.Request.Host}/mcp";
        }

        return Results.Json(new
        {
            resource,
            authorization_servers = new[] { authority },
            scopes_supported = Scopes,
            bearer_methods_supported = new[] { "header" }
        });
    }

    private static string? NormalizeAuthority(string? authority)
    {
        if (string.IsNullOrWhiteSpace(authority))
        {
            return null;
        }

        return authority.Trim().TrimEnd('/') + "/";
    }
}
