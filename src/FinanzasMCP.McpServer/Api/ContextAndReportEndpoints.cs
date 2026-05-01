using FinanzasMCP.Application.Reports.Handlers;
using FinanzasMCP.Application.Reports.Queries;
using FinanzasMCP.Application.UserContext.Commands;
using FinanzasMCP.Application.UserContext.Handlers;
using FinanzasMCP.Application.UserContext.Queries;

namespace FinanzasMCP.McpServer.Api;

public static class ContextAndReportEndpoints
{
    public static void MapContextAndReportEndpoints(this RouteGroupBuilder api)
    {
        MapUserContext(api.MapGroup("/user-context"));
        MapReports(api.MapGroup("/reports"));
    }

    private static void MapUserContext(RouteGroupBuilder group)
    {
        group.MapGet("", async (GetUserContextHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetUserContextQuery(), ct)));

        group.MapPut("{key}", async (string key, UpsertUserContextEntryRequest request, UpsertUserContextEntryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpsertUserContextEntryCommand(key, request.Value), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{key}", async (string key, DeleteUserContextEntryHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteUserContextEntryCommand(key), ct);
            return Results.NoContent();
        });
    }

    private static void MapReports(RouteGroupBuilder group)
    {
        group.MapGet("finance-overview", async (GetFinanceOverviewHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetFinanceOverviewQuery(), ct)));
    }
}
