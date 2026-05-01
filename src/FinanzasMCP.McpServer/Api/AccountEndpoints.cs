using FinanzasMCP.Application.Accounts.Commands;
using FinanzasMCP.Application.Accounts.Handlers;
using FinanzasMCP.Application.Accounts.Queries;

namespace FinanzasMCP.McpServer.Api;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/accounts");

        group.MapGet("", async (GetAccountsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetAccountsQuery(), ct)));

        group.MapPost("", async (CreateAccountRequest request, CreateAccountHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateAccountCommand(request.Name, request.AccountType, request.Currency, request.Balance, request.BankName, request.AccountNumber, request.Provider, request.CryptoSymbol, request.CryptoNetwork, request.CryptoQuantity, request.CryptoAvgBuyPriceUsd), ct);
            return Results.Created($"/api/v1/accounts/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateAccountRequest request, UpdateAccountHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateAccountCommand(id, request.Name, request.Currency, request.BankName, request.AccountNumber, request.Provider, request.IsActive), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteAccountHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteAccountCommand(id), ct);
            return Results.NoContent();
        });
    }
}
