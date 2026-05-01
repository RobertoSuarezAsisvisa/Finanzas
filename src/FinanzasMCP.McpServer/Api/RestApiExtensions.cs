namespace FinanzasMCP.McpServer.Api;

public static class RestApiExtensions
{
    public static IEndpointRouteBuilder MapFinanzasRestApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/v1");

        api.MapGet("/health", () => Results.Ok(new { status = "ok" }));

        api.MapAccountEndpoints();
        api.MapCatalogEndpoints();
        api.MapCashflowEndpoints();
        api.MapCryptoEndpoints();
        api.MapAccountingPeriodEndpoints();
        api.MapGoalEndpoints();
        api.MapDebtEndpoints();
        api.MapRecurringRuleEndpoints();
        api.MapContextAndReportEndpoints();

        return app;
    }
}
