using FinanzasMCP.Application.AccountingPeriods.Commands;
using FinanzasMCP.Application.AccountingPeriods.Handlers;
using FinanzasMCP.Application.AccountingPeriods.Queries;
using FinanzasMCP.Domain.AccountingPeriods;

namespace FinanzasMCP.McpServer.Api;

public static class AccountingPeriodEndpoints
{
    public static void MapAccountingPeriodEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/accounting-periods");

        group.MapGet("", async (AccountingPeriodStatus? status, GetAccountingPeriodsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetAccountingPeriodsQuery(status), ct)));

        group.MapPost("", async (CreateAccountingPeriodRequest request, CreateAccountingPeriodHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateAccountingPeriodCommand(request.Name, request.StartDate, request.EndDate), ct);
            return Results.Created($"/api/v1/accounting-periods/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateAccountingPeriodRequest request, UpdateAccountingPeriodHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateAccountingPeriodCommand(id, request.Name, request.StartDate, request.EndDate, request.Status, request.TotalIncome, request.TotalExpenses, request.NetBalance, request.ClosedAt), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteAccountingPeriodHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteAccountingPeriodCommand(id), ct);
            return Results.NoContent();
        });
    }
}
