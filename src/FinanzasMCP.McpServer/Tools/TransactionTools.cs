using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Transactions.Commands;
using FinanzasMCP.Application.Transactions.Handlers;
using FinanzasMCP.Application.Transactions.Queries;
using FinanzasMCP.Domain.Transactions;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class TransactionTools(
    CreateTransactionHandler createTransactionHandler,
    GetTransactionsHandler getTransactionsHandler,
    UpdateTransactionHandler updateTransactionHandler,
    DeleteTransactionHandler deleteTransactionHandler)
{
    [McpServerTool, System.ComponentModel.Description("Registers an income, expense, or transfer.")]
    public Task<TransactionSummary> CreateTransaction(
        TransactionType type,
        decimal amount,
        string currency,
        Guid accountId,
        Guid? toAccountId = null,
        Guid? categoryId = null,
        string? description = null,
        string? reference = null,
        DateTimeOffset? transactionDate = null,
        Guid? recurringRuleId = null,
        IReadOnlyList<Guid>? tagIds = null,
        CancellationToken cancellationToken = default)
        => createTransactionHandler.Handle(new CreateTransactionCommand(type, amount, currency, accountId, toAccountId, categoryId, description, reference, transactionDate ?? DateTimeOffset.UtcNow, recurringRuleId, tagIds ?? Array.Empty<Guid>()), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists transactions, optionally filtered by account.")]
    public Task<IReadOnlyList<TransactionSummary>> ListTransactions(Guid? accountId = null, CancellationToken cancellationToken = default)
        => getTransactionsHandler.Handle(new GetTransactionsQuery(accountId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a transaction and reapplies its balance impact.")]
    public Task<TransactionSummary> UpdateTransaction(
        Guid id,
        TransactionType type,
        decimal amount,
        string currency,
        Guid accountId,
        Guid? toAccountId = null,
        Guid? categoryId = null,
        string? description = null,
        string? reference = null,
        DateTimeOffset? transactionDate = null,
        Guid? recurringRuleId = null,
        IReadOnlyList<Guid>? tagIds = null,
        CancellationToken cancellationToken = default)
        => updateTransactionHandler.Handle(new UpdateTransactionCommand(id, type, amount, currency, accountId, toAccountId, categoryId, description, reference, transactionDate ?? DateTimeOffset.UtcNow, recurringRuleId, tagIds ?? Array.Empty<Guid>()), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a transaction and reverts its balance impact.")]
    public Task DeleteTransaction(Guid id, CancellationToken cancellationToken = default)
        => deleteTransactionHandler.Handle(new DeleteTransactionCommand(id), cancellationToken);
}
