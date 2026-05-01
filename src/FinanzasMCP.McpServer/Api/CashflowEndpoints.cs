using FinanzasMCP.Application.Budgets.Commands;
using FinanzasMCP.Application.Budgets.Handlers;
using FinanzasMCP.Application.Budgets.Queries;
using FinanzasMCP.Application.Transactions.Commands;
using FinanzasMCP.Application.Transactions.Handlers;
using FinanzasMCP.Application.Transactions.Queries;

namespace FinanzasMCP.McpServer.Api;

public static class CashflowEndpoints
{
    public static void MapCashflowEndpoints(this RouteGroupBuilder api)
    {
        MapTransactions(api.MapGroup("/transactions"));
        MapBudgets(api.MapGroup("/budgets"));
    }

    private static void MapTransactions(RouteGroupBuilder group)
    {
        group.MapGet("", async (Guid? accountId, GetTransactionsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetTransactionsQuery(accountId), ct)));

        group.MapPost("", async (CreateTransactionRequest request, CreateTransactionHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateTransactionCommand(request.Type, request.Amount, request.Currency, request.AccountId, request.ToAccountId, request.CategoryId, request.Description, request.Reference, request.TransactionDate, request.RecurringRuleId, request.TagIds ?? Array.Empty<Guid>()), ct);
            return Results.Created($"/api/v1/transactions/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateTransactionRequest request, UpdateTransactionHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateTransactionCommand(id, request.Type, request.Amount, request.Currency, request.AccountId, request.ToAccountId, request.CategoryId, request.Description, request.Reference, request.TransactionDate, request.RecurringRuleId, request.TagIds ?? Array.Empty<Guid>()), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteTransactionHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteTransactionCommand(id), ct);
            return Results.NoContent();
        });
    }

    private static void MapBudgets(RouteGroupBuilder group)
    {
        group.MapGet("", async (GetBudgetsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetBudgetsQuery(), ct)));

        group.MapPost("", async (CreateBudgetRequest request, CreateBudgetHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateBudgetCommand(request.Name, request.CategoryId, request.LimitAmount, request.PeriodType, request.ValidityType, request.PeriodStart, request.PeriodEnd), ct);
            return Results.Created($"/api/v1/budgets/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateBudgetRequest request, UpdateBudgetHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateBudgetCommand(id, request.Name, request.LimitAmount, request.PeriodType, request.ValidityType, request.PeriodStart, request.PeriodEnd, request.IsActive), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteBudgetHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteBudgetCommand(id), ct);
            return Results.NoContent();
        });
    }
}
