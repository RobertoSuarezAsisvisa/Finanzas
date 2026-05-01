using FinanzasMCP.Application.RecurringRules.Commands;
using FinanzasMCP.Application.RecurringRules.Handlers;
using FinanzasMCP.Application.RecurringRules.Queries;

namespace FinanzasMCP.McpServer.Api;

public static class RecurringRuleEndpoints
{
    public static void MapRecurringRuleEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/recurring-rules");

        group.MapGet("", async (GetRecurringRulesHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetRecurringRulesQuery(), ct)));

        group.MapPost("", async (CreateRecurringRuleRequest request, CreateRecurringRuleHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateRecurringRuleCommand(request.Name, request.Type, request.Amount, request.AccountId, request.CategoryId, request.Frequency, request.StartDate, request.EndDate, request.NextDueDate), ct);
            return Results.Created($"/api/v1/recurring-rules/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateRecurringRuleRequest request, UpdateRecurringRuleHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateRecurringRuleCommand(id, request.Name, request.Type, request.Amount, request.AccountId, request.CategoryId, request.Frequency, request.StartDate, request.EndDate, request.NextDueDate, request.IsActive), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteRecurringRuleHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteRecurringRuleCommand(id), ct);
            return Results.NoContent();
        });
    }
}
