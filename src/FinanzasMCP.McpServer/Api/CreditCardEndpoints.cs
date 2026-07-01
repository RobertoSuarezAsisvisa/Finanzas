using FinanzasMCP.Application.CreditCards.Commands;
using FinanzasMCP.Application.CreditCards.Handlers;
using FinanzasMCP.Application.CreditCards.Queries;
using FinanzasMCP.Domain.CreditCards;

namespace FinanzasMCP.McpServer.Api;

public static class CreditCardEndpoints
{
    public static void MapCreditCardEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/credit-cards");

        group.MapGet("", async (GetCreditCardsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetCreditCardsQuery(), ct)));

        group.MapPost("", async (CreateCreditCardRequest request, CreateCreditCardHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateCreditCardCommand(
                request.Name,
                request.Currency,
                request.Issuer,
                request.Brand,
                request.CreditLimit,
                request.StatementClosingDay,
                request.PaymentDueDay,
                request.ProductName,
                request.LastFour,
                request.OutstandingBalance ?? 0m,
                request.PaymentMode ?? CreditCardPaymentMode.Manual,
                request.RewardsProgram,
                request.StatementDelivery ?? CreditCardStatementDelivery.Virtual,
                request.InterestNominalAnnual,
                request.InterestEffectiveAnnual), ct);
            return Results.Created($"/api/v1/credit-cards/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateCreditCardRequest request, UpdateCreditCardHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new UpdateCreditCardCommand(
                id,
                request.Name,
                request.Currency,
                request.Issuer,
                request.Brand,
                request.CreditLimit,
                request.StatementClosingDay,
                request.PaymentDueDay,
                request.ProductName,
                request.LastFour,
                request.PaymentMode ?? CreditCardPaymentMode.Manual,
                request.RewardsProgram,
                request.StatementDelivery ?? CreditCardStatementDelivery.Virtual,
                request.InterestNominalAnnual,
                request.InterestEffectiveAnnual,
                request.IsActive), ct)));

        group.MapDelete("{id:guid}", async (Guid id, DeleteCreditCardHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteCreditCardCommand(id), ct);
            return Results.NoContent();
        });

        group.MapGet("{id:guid}/statements", async (Guid id, GetCreditCardStatementsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetCreditCardStatementsQuery(id), ct)));

        group.MapPost("{id:guid}/statements", async (Guid id, CloseCreditCardStatementRequest request, CloseCreditCardStatementHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CloseCreditCardStatementCommand(id, request.PeriodStart, request.PeriodEnd, request.StatementDate, request.DueDate, request.MinimumPayment), ct);
            return Results.Created($"/api/v1/credit-cards/{id}/statements/{result.Id}", result);
        });
    }
}
