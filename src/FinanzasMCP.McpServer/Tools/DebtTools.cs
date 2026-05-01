using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Debts.Handlers;
using FinanzasMCP.Application.Debts.Queries;
using FinanzasMCP.Domain.Debts;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class DebtTools(
    CreateDebtHandler createDebtHandler,
    RegisterDebtPaymentHandler registerDebtPaymentHandler,
    UpdateDebtHandler updateDebtHandler,
    DeleteDebtHandler deleteDebtHandler,
    GetDebtsHandler getDebtsHandler)
{
    [McpServerTool, System.ComponentModel.Description("Creates a debt.")]
    public Task<DebtSummary> CreateDebt(DebtType type, string contactName, decimal originalAmount, decimal remainingAmount, string currency = "USD", DateTimeOffset? dueDate = null, Guid? accountId = null, string? notes = null, CancellationToken cancellationToken = default)
        => createDebtHandler.Handle(new CreateDebtCommand(type, contactName, originalAmount, remainingAmount, currency, dueDate, accountId, notes), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists debts.")]
    public Task<IReadOnlyList<DebtSummary>> ListDebts(CancellationToken cancellationToken = default)
        => getDebtsHandler.Handle(new GetDebtsQuery(), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Registers a payment for a debt.")]
    public Task<DebtSummary> RegisterDebtPayment(Guid debtId, decimal amount, DateTimeOffset paymentDate, string? notes = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => registerDebtPaymentHandler.Handle(new RegisterDebtPaymentCommand(debtId, amount, paymentDate, notes, transactionId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a debt.")]
    public Task<DebtSummary> UpdateDebt(Guid id, DebtType type, string contactName, decimal originalAmount, decimal remainingAmount, string currency = "USD", DateTimeOffset? dueDate = null, Guid? accountId = null, DebtStatus? status = null, string? notes = null, CancellationToken cancellationToken = default)
        => updateDebtHandler.Handle(new UpdateDebtCommand(id, type, contactName, originalAmount, remainingAmount, currency, dueDate, accountId, status, notes), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a debt.")]
    public Task DeleteDebt(Guid id, CancellationToken cancellationToken = default)
        => deleteDebtHandler.Handle(new DeleteDebtCommand(id), cancellationToken);
}
