using FinanzasMCP.Application.Goals.Commands;
using FinanzasMCP.Application.Goals.Handlers;
using FinanzasMCP.Application.Goals.Queries;
using FinanzasMCP.Domain.Goals;

namespace FinanzasMCP.McpServer.Api;

public static class GoalEndpoints
{
    public static void MapGoalEndpoints(this RouteGroupBuilder api)
    {
        MapGoals(api.MapGroup("/goals"));
        MapGoalContributions(api.MapGroup("/goal-contributions"));
        MapLegacySavingGoals(api.MapGroup("/saving-goals"));
        MapLegacySavingGoalContributions(api.MapGroup("/saving-goal-contributions"));
        MapLegacyPurchaseGoals(api.MapGroup("/purchase-goals"));
        MapLegacyPurchaseGoalContributions(api.MapGroup("/purchase-goal-contributions"));
    }

    private static void MapGoals(RouteGroupBuilder group)
    {
        group.MapGet("", async (FinancialGoalType? type, GetFinancialGoalsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetFinancialGoalsQuery(type), ct)));

        group.MapPost("", async (CreateFinancialGoalRequest request, CreateFinancialGoalHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateFinancialGoalCommand(request.Name, request.TargetAmount, request.Type, request.Description, request.Priority, request.Url, request.AccountId, request.TargetDate), ct);
            return Results.Created($"/api/v1/goals/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateFinancialGoalRequest request, UpdateFinancialGoalHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateFinancialGoalCommand(id, request.Name, request.TargetAmount, request.Type, request.Description, request.Priority, request.Url, request.AccountId, request.TargetDate, request.Status, request.CompletedAt), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteFinancialGoalHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteFinancialGoalCommand(id), ct);
            return Results.NoContent();
        });

        group.MapPost("{goalId:guid}/contributions", async (Guid goalId, AddFinancialGoalContributionRequest request, AddFinancialGoalContributionHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new AddFinancialGoalContributionCommand(goalId, request.Amount, request.ContributionDate, request.TransactionId, request.AccountId), ct);
            return Results.Created($"/api/v1/goals/{goalId}/contributions", result);
        });
    }

    private static void MapGoalContributions(RouteGroupBuilder group)
    {
        group.MapGet("", async (Guid? goalId, GetFinancialGoalContributionsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetFinancialGoalContributionsQuery(goalId), ct)));

        group.MapPut("{id:guid}", async (Guid id, UpdateFinancialGoalContributionRequest request, UpdateFinancialGoalContributionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new UpdateFinancialGoalContributionCommand(id, request.Amount, request.ContributionDate, request.TransactionId, request.AccountId), ct);
            return Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteFinancialGoalContributionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteFinancialGoalContributionCommand(id), ct);
            return Results.NoContent();
        });
    }

    private static void MapLegacySavingGoals(RouteGroupBuilder group)
    {
        group.MapGet("", async (GetFinancialGoalsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetFinancialGoalsQuery(FinancialGoalType.Saving), ct)));

        group.MapPost("", async (CreateSavingGoalRequest request, CreateFinancialGoalHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateFinancialGoalCommand(request.Name, request.TargetAmount, FinancialGoalType.Saving, AccountId: request.AccountId, TargetDate: request.TargetDate), ct);
            return Results.Created($"/api/v1/saving-goals/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateSavingGoalRequest request, UpdateFinancialGoalHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateFinancialGoalCommand(id, request.Name, request.TargetAmount, FinancialGoalType.Saving, AccountId: request.AccountId, TargetDate: request.TargetDate, Status: MapSavingStatus(request.Status)), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteFinancialGoalHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteFinancialGoalCommand(id), ct);
            return Results.NoContent();
        });

        group.MapPost("{goalId:guid}/contributions", async (Guid goalId, AddSavingGoalContributionRequest request, AddFinancialGoalContributionHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new AddFinancialGoalContributionCommand(goalId, request.Amount, request.ContributionDate, request.TransactionId, request.AccountId), ct);
            return Results.Created($"/api/v1/saving-goals/{goalId}/contributions", result);
        });
    }

    private static void MapLegacySavingGoalContributions(RouteGroupBuilder group)
    {
        group.MapGet("", async (Guid? goalId, GetFinancialGoalContributionsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetFinancialGoalContributionsQuery(goalId), ct)));

        group.MapPut("{id:guid}", async (Guid id, UpdateSavingGoalContributionRequest request, UpdateFinancialGoalContributionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new UpdateFinancialGoalContributionCommand(id, request.Amount, request.ContributionDate, request.TransactionId, request.AccountId), ct);
            return Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteFinancialGoalContributionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteFinancialGoalContributionCommand(id), ct);
            return Results.NoContent();
        });
    }

    private static void MapLegacyPurchaseGoals(RouteGroupBuilder group)
    {
        group.MapGet("", async (GetFinancialGoalsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetFinancialGoalsQuery(FinancialGoalType.Purchase), ct)));

        group.MapPost("", async (CreatePurchaseGoalRequest request, CreateFinancialGoalHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateFinancialGoalCommand(request.Name, request.TargetPrice, FinancialGoalType.Purchase, request.Description, request.Priority, request.Url, request.AccountId, request.TargetDate), ct);
            return Results.Created($"/api/v1/purchase-goals/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdatePurchaseGoalRequest request, UpdateFinancialGoalHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateFinancialGoalCommand(id, request.Name, request.TargetPrice, FinancialGoalType.Purchase, request.Description, request.Priority, request.Url, request.AccountId, request.TargetDate, MapPurchaseStatus(request.Status), request.PurchasedAt), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteFinancialGoalHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteFinancialGoalCommand(id), ct);
            return Results.NoContent();
        });

        group.MapPost("{purchaseGoalId:guid}/contributions", async (Guid purchaseGoalId, AddPurchaseGoalContributionRequest request, AddFinancialGoalContributionHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new AddFinancialGoalContributionCommand(purchaseGoalId, request.Amount, request.ContributionDate, request.TransactionId, request.AccountId), ct);
            return Results.Created($"/api/v1/purchase-goals/{purchaseGoalId}/contributions", result);
        });
    }

    private static void MapLegacyPurchaseGoalContributions(RouteGroupBuilder group)
    {
        group.MapGet("", async (Guid? purchaseGoalId, GetFinancialGoalContributionsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetFinancialGoalContributionsQuery(purchaseGoalId), ct)));

        group.MapPut("{id:guid}", async (Guid id, UpdatePurchaseGoalContributionRequest request, UpdateFinancialGoalContributionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new UpdateFinancialGoalContributionCommand(id, request.Amount, request.ContributionDate, request.TransactionId, request.AccountId), ct);
            return Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteFinancialGoalContributionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteFinancialGoalContributionCommand(id), ct);
            return Results.NoContent();
        });
    }

    private static FinancialGoalStatus? MapSavingStatus(SavingGoalStatus? status)
        => status switch
        {
            null => null,
            SavingGoalStatus.InProgress => FinancialGoalStatus.InProgress,
            SavingGoalStatus.Completed => FinancialGoalStatus.Completed,
            SavingGoalStatus.Cancelled => FinancialGoalStatus.Cancelled,
            _ => throw new InvalidOperationException("Unsupported saving goal status.")
        };

    private static FinancialGoalStatus? MapPurchaseStatus(PurchaseGoalStatus? status)
        => status switch
        {
            null => null,
            PurchaseGoalStatus.Saving => FinancialGoalStatus.InProgress,
            PurchaseGoalStatus.Ready => FinancialGoalStatus.Ready,
            PurchaseGoalStatus.Purchased => FinancialGoalStatus.Completed,
            PurchaseGoalStatus.Cancelled => FinancialGoalStatus.Cancelled,
            _ => throw new InvalidOperationException("Unsupported purchase goal status.")
        };
}
