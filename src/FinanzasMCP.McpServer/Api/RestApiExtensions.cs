namespace FinanzasMCP.McpServer.Api;

public static class RestApiExtensions
{
    public static IEndpointRouteBuilder MapFinanzasRestApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/v1");

        api.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();
        api.MapAuthEndpoints();

        var secureApi = api.MapGroup("").RequireAuthorization();

        secureApi.MapAccountEndpoints();
        secureApi.MapCatalogEndpoints();
        secureApi.MapCashflowEndpoints();
        secureApi.MapCryptoEndpoints();
        secureApi.MapAccountingPeriodEndpoints();
        secureApi.MapGoalEndpoints();
        secureApi.MapDebtEndpoints();
        secureApi.MapRecurringRuleEndpoints();
        secureApi.MapContextAndReportEndpoints();
        secureApi.MapShoppingEndpoints();

        return app;
    }
}
