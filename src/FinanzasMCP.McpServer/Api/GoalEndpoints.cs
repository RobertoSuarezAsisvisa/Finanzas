using FinanzasMCP.Application.PurchaseGoals.Commands;
using FinanzasMCP.Application.PurchaseGoals.Handlers;
using FinanzasMCP.Application.PurchaseGoals.Queries;
using FinanzasMCP.Application.SavingGoals.Commands;
using FinanzasMCP.Application.SavingGoals.Handlers;
using FinanzasMCP.Application.SavingGoals.Queries;

namespace FinanzasMCP.McpServer.Api;

public static class GoalEndpoints
{
    public static void MapGoalEndpoints(this RouteGroupBuilder api)
    {
        MapSavingGoals(api.MapGroup("/saving-goals"));
        MapSavingGoalContributions(api.MapGroup("/saving-goal-contributions"));
        MapPurchaseGoals(api.MapGroup("/purchase-goals"));
        MapPurchaseGoalContributions(api.MapGroup("/purchase-goal-contributions"));
    }

    private static void MapSavingGoals(RouteGroupBuilder group)
    {
        group.MapGet("", async (GetSavingGoalsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetSavingGoalsQuery(), ct)));

        group.MapPost("", async (CreateSavingGoalRequest request, CreateSavingGoalHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateSavingGoalCommand(request.Name, request.TargetAmount, request.AccountId, request.TargetDate), ct);
            return Results.Created($"/api/v1/saving-goals/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateSavingGoalRequest request, UpdateSavingGoalHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateSavingGoalCommand(id, request.Name, request.TargetAmount, request.AccountId, request.TargetDate, request.Status), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteSavingGoalHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteSavingGoalCommand(id), ct);
            return Results.NoContent();
        });

        group.MapPost("{goalId:guid}/contributions", async (Guid goalId, AddSavingGoalContributionRequest request, AddSavingGoalContributionHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new AddSavingGoalContributionCommand(goalId, request.Amount, request.ContributionDate, request.TransactionId), ct);
            return Results.Created($"/api/v1/saving-goals/{goalId}/contributions", result);
        });
    }

    private static void MapSavingGoalContributions(RouteGroupBuilder group)
    {
        group.MapGet("", async (Guid? goalId, GetSavingGoalContributionsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetSavingGoalContributionsQuery(goalId), ct)));

        group.MapPut("{id:guid}", async (Guid id, UpdateSavingGoalContributionRequest request, UpdateSavingGoalContributionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new UpdateSavingGoalContributionCommand(id, request.Amount, request.ContributionDate, request.TransactionId), ct);
            return Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteSavingGoalContributionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteSavingGoalContributionCommand(id), ct);
            return Results.NoContent();
        });
    }

    private static void MapPurchaseGoals(RouteGroupBuilder group)
    {
        group.MapGet("", async (GetPurchaseGoalsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetPurchaseGoalsQuery(), ct)));

        group.MapPost("", async (CreatePurchaseGoalRequest request, CreatePurchaseGoalHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreatePurchaseGoalCommand(request.Name, request.TargetPrice, request.Description, request.Priority, request.Url, request.AccountId, request.TargetDate), ct);
            return Results.Created($"/api/v1/purchase-goals/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdatePurchaseGoalRequest request, UpdatePurchaseGoalHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdatePurchaseGoalCommand(id, request.Name, request.TargetPrice, request.Description, request.Priority, request.Url, request.AccountId, request.TargetDate, request.Status, request.PurchasedAt), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeletePurchaseGoalHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeletePurchaseGoalCommand(id), ct);
            return Results.NoContent();
        });

        group.MapPost("{purchaseGoalId:guid}/contributions", async (Guid purchaseGoalId, AddPurchaseGoalContributionRequest request, AddPurchaseGoalContributionHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new AddPurchaseGoalContributionCommand(purchaseGoalId, request.Amount, request.ContributionDate, request.TransactionId), ct);
            return Results.Created($"/api/v1/purchase-goals/{purchaseGoalId}/contributions", result);
        });
    }

    private static void MapPurchaseGoalContributions(RouteGroupBuilder group)
    {
        group.MapGet("", async (Guid? purchaseGoalId, GetPurchaseGoalContributionsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetPurchaseGoalContributionsQuery(purchaseGoalId), ct)));

        group.MapPut("{id:guid}", async (Guid id, UpdatePurchaseGoalContributionRequest request, UpdatePurchaseGoalContributionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new UpdatePurchaseGoalContributionCommand(id, request.Amount, request.ContributionDate, request.TransactionId), ct);
            return Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, DeletePurchaseGoalContributionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeletePurchaseGoalContributionCommand(id), ct);
            return Results.NoContent();
        });
    }
}
