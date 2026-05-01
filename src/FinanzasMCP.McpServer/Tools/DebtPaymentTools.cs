using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Debts.Commands;
using FinanzasMCP.Application.Debts.Handlers;
using FinanzasMCP.Application.Debts.Queries;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class DebtPaymentTools(
    RegisterDebtPaymentHandler registerDebtPaymentHandler,
    UpdateDebtPaymentHandler updateDebtPaymentHandler,
    DeleteDebtPaymentHandler deleteDebtPaymentHandler,
    GetDebtPaymentsHandler getDebtPaymentsHandler)
{
    [McpServerTool, System.ComponentModel.Description("Registers a payment for a debt.")]
    public Task<DebtSummary> RegisterDebtPayment(Guid debtId, decimal amount, DateTimeOffset paymentDate, string? notes = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => registerDebtPaymentHandler.Handle(new RegisterDebtPaymentCommand(debtId, amount, paymentDate, notes, transactionId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists debt payments, optionally filtered by debt.")]
    public Task<IReadOnlyList<DebtPaymentSummary>> ListDebtPayments(Guid? debtId = null, CancellationToken cancellationToken = default)
        => getDebtPaymentsHandler.Handle(new GetDebtPaymentsQuery(debtId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a debt payment.")]
    public Task UpdateDebtPayment(Guid id, decimal amount, DateTimeOffset paymentDate, string? notes = null, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => updateDebtPaymentHandler.Handle(new UpdateDebtPaymentCommand(id, amount, paymentDate, notes, transactionId), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a debt payment.")]
    public Task DeleteDebtPayment(Guid id, CancellationToken cancellationToken = default)
        => deleteDebtPaymentHandler.Handle(new DeleteDebtPaymentCommand(id), cancellationToken);
}
