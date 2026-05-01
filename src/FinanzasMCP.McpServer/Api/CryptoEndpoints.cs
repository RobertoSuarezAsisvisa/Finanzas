using FinanzasMCP.Application.Crypto.Commands;
using FinanzasMCP.Application.Crypto.Handlers;
using FinanzasMCP.Application.Crypto.Queries;
using FinanzasMCP.Application.CryptoAccounts.Commands;
using FinanzasMCP.Application.CryptoAccounts.Handlers;
using FinanzasMCP.Application.CryptoAccounts.Queries;

namespace FinanzasMCP.McpServer.Api;

public static class CryptoEndpoints
{
    public static void MapCryptoEndpoints(this RouteGroupBuilder api)
    {
        MapCryptoAccounts(api.MapGroup("/crypto-accounts"));
        MapCryptoLots(api.MapGroup("/crypto-lots"));
    }

    private static void MapCryptoAccounts(RouteGroupBuilder group)
    {
        group.MapGet("", async (Guid? accountId, GetCryptoAccountsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetCryptoAccountsQuery(accountId), ct)));

        group.MapPost("", async (CreateCryptoAccountRequest request, CreateCryptoAccountHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateCryptoAccountCommand(request.AccountId, request.Symbol, request.Network, request.Quantity, request.AvgBuyPriceUsd), ct);
            return Results.Created($"/api/v1/crypto-accounts/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateCryptoAccountRequest request, UpdateCryptoAccountHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateCryptoAccountCommand(id, request.AccountId, request.Symbol, request.Network, request.Quantity, request.AvgBuyPriceUsd), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteCryptoAccountHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteCryptoAccountCommand(id), ct);
            return Results.NoContent();
        });
    }

    private static void MapCryptoLots(RouteGroupBuilder group)
    {
        group.MapGet("", async (Guid? accountId, GetCryptoLotsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetCryptoLotsQuery(accountId), ct)));

        group.MapPost("", async (CreateCryptoLotRequest request, CreateCryptoLotHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateCryptoLotCommand(request.AccountId, request.TransactionId, request.Quantity, request.BuyPriceUsd, request.SellPriceUsd, request.Status, request.OperationDate ?? DateTimeOffset.UtcNow), ct);
            return Results.Created($"/api/v1/crypto-lots/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateCryptoLotRequest request, UpdateCryptoLotHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateCryptoLotCommand(id, request.AccountId, request.TransactionId, request.Quantity, request.BuyPriceUsd, request.SellPriceUsd, request.Status, request.OperationDate ?? DateTimeOffset.UtcNow), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteCryptoLotHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteCryptoLotCommand(id), ct);
            return Results.NoContent();
        });
    }
}
