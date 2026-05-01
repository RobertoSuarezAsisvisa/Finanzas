using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Crypto.Commands;
using FinanzasMCP.Application.Crypto.Handlers;
using FinanzasMCP.Application.Crypto.Queries;
using FinanzasMCP.Domain.Crypto;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class CryptoLotTools(
    CreateCryptoLotHandler createCryptoLotHandler,
    GetCryptoLotsHandler getCryptoLotsHandler,
    UpdateCryptoLotHandler updateCryptoLotHandler,
    DeleteCryptoLotHandler deleteCryptoLotHandler)
{
    [McpServerTool, System.ComponentModel.Description("Creates a crypto lot.")]
    public Task<CryptoLotSummary> CreateCryptoLot(Guid accountId, decimal quantity, decimal buyPriceUsd, CryptoLotStatus status = CryptoLotStatus.Open, Guid? transactionId = null, decimal? sellPriceUsd = null, DateTimeOffset? operationDate = null, CancellationToken cancellationToken = default)
        => createCryptoLotHandler.Handle(new CreateCryptoLotCommand(accountId, transactionId, quantity, buyPriceUsd, sellPriceUsd, status, operationDate ?? DateTimeOffset.UtcNow), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists crypto lots.")]
    public Task<IReadOnlyList<CryptoLotSummary>> ListCryptoLots(Guid? accountId = null, CancellationToken cancellationToken = default)
        => getCryptoLotsHandler.Handle(new GetCryptoLotsQuery(accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a crypto lot.")]
    public Task<CryptoLotSummary> UpdateCryptoLot(Guid id, Guid accountId, decimal quantity, decimal buyPriceUsd, CryptoLotStatus status, Guid? transactionId = null, decimal? sellPriceUsd = null, DateTimeOffset? operationDate = null, CancellationToken cancellationToken = default)
        => updateCryptoLotHandler.Handle(new UpdateCryptoLotCommand(id, accountId, transactionId, quantity, buyPriceUsd, sellPriceUsd, status, operationDate ?? DateTimeOffset.UtcNow), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a crypto lot.")]
    public Task DeleteCryptoLot(Guid id, CancellationToken cancellationToken = default)
        => deleteCryptoLotHandler.Handle(new DeleteCryptoLotCommand(id), cancellationToken);
}
