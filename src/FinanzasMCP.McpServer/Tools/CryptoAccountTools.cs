using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.CryptoAccounts.Commands;
using FinanzasMCP.Application.CryptoAccounts.Handlers;
using FinanzasMCP.Application.CryptoAccounts.Queries;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class CryptoAccountTools(
    CreateCryptoAccountHandler createCryptoAccountHandler,
    GetCryptoAccountsHandler getCryptoAccountsHandler,
    UpdateCryptoAccountHandler updateCryptoAccountHandler,
    DeleteCryptoAccountHandler deleteCryptoAccountHandler)
{
    [McpServerTool, System.ComponentModel.Description("Creates crypto account details for an account.")]
    public Task<CryptoAccountSummary> CreateCryptoAccount(Guid accountId, string symbol, string? network = null, decimal quantity = 0m, decimal? avgBuyPriceUsd = null, CancellationToken cancellationToken = default)
        => createCryptoAccountHandler.Handle(new CreateCryptoAccountCommand(accountId, symbol, network, quantity, avgBuyPriceUsd), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists crypto accounts.")]
    public Task<IReadOnlyList<CryptoAccountSummary>> ListCryptoAccounts(Guid? accountId = null, CancellationToken cancellationToken = default)
        => getCryptoAccountsHandler.Handle(new GetCryptoAccountsQuery(accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates crypto account details.")]
    public Task<CryptoAccountSummary> UpdateCryptoAccount(Guid id, Guid accountId, string symbol, string? network = null, decimal quantity = 0m, decimal? avgBuyPriceUsd = null, CancellationToken cancellationToken = default)
        => updateCryptoAccountHandler.Handle(new UpdateCryptoAccountCommand(id, accountId, symbol, network, quantity, avgBuyPriceUsd), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes crypto account details.")]
    public Task DeleteCryptoAccount(Guid id, CancellationToken cancellationToken = default)
        => deleteCryptoAccountHandler.Handle(new DeleteCryptoAccountCommand(id), cancellationToken);
}
