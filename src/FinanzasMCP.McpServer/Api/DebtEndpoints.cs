using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Debts.Handlers;
using FinanzasMCP.Application.Debts.Queries;

namespace FinanzasMCP.McpServer.Api;

public static class DebtEndpoints
{
    public static void MapDebtEndpoints(this RouteGroupBuilder api)
    {
        MapDebts(api.MapGroup("/debts"));
        MapDebtPayments(api.MapGroup("/debt-payments"));
    }

    private static void MapDebts(RouteGroupBuilder group)
    {
        group.MapGet("", async (GetDebtsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetDebtsQuery(), ct)));

        group.MapPost("", async (CreateDebtRequest request, CreateDebtHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateDebtCommand(request.Type, request.ContactName, request.OriginalAmount, request.RemainingAmount, request.Currency, request.DueDate, request.AccountId, request.Notes), ct);
            return Results.Created($"/api/v1/debts/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateDebtRequest request, UpdateDebtHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateDebtCommand(id, request.Type, request.ContactName, request.OriginalAmount, request.RemainingAmount, request.Currency, request.DueDate, request.AccountId, request.Status, request.Notes), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteDebtHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteDebtCommand(id), ct);
            return Results.NoContent();
        });

        group.MapPost("{debtId:guid}/payments", async (Guid debtId, RegisterDebtPaymentRequest request, RegisterDebtPaymentHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new RegisterDebtPaymentCommand(debtId, request.Amount, request.PaymentDate, request.Notes, request.TransactionId), ct);
            return Results.Created($"/api/v1/debts/{debtId}/payments", result);
        });
    }

    private static void MapDebtPayments(RouteGroupBuilder group)
    {
        group.MapGet("", async (Guid? debtId, GetDebtPaymentsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetDebtPaymentsQuery(debtId), ct)));

        group.MapPut("{id:guid}", async (Guid id, UpdateDebtPaymentRequest request, UpdateDebtPaymentHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new UpdateDebtPaymentCommand(id, request.Amount, request.PaymentDate, request.Notes, request.TransactionId), ct);
            return Results.NoContent();
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteDebtPaymentHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteDebtPaymentCommand(id), ct);
            return Results.NoContent();
        });
    }
}
