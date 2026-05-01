using FinanzasMCP.Application.AccountingPeriods.Commands;
using FinanzasMCP.Application.AccountingPeriods.Handlers;
using FinanzasMCP.Application.AccountingPeriods.Queries;
using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Domain.AccountingPeriods;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class AccountingPeriodTools(
    CreateAccountingPeriodHandler createAccountingPeriodHandler,
    GetAccountingPeriodsHandler getAccountingPeriodsHandler,
    UpdateAccountingPeriodHandler updateAccountingPeriodHandler,
    DeleteAccountingPeriodHandler deleteAccountingPeriodHandler)
{
    [McpServerTool, System.ComponentModel.Description("Creates an accounting period.")]
    public Task<AccountingPeriodSummary> CreateAccountingPeriod(string name, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
        => createAccountingPeriodHandler.Handle(new CreateAccountingPeriodCommand(name, startDate, endDate), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists accounting periods.")]
    public Task<IReadOnlyList<AccountingPeriodSummary>> ListAccountingPeriods(AccountingPeriodStatus? status = null, CancellationToken cancellationToken = default)
        => getAccountingPeriodsHandler.Handle(new GetAccountingPeriodsQuery(status), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates an accounting period.")]
    public Task<AccountingPeriodSummary> UpdateAccountingPeriod(Guid id, string name, DateTimeOffset startDate, DateTimeOffset endDate, AccountingPeriodStatus status, decimal totalIncome = 0m, decimal totalExpenses = 0m, decimal netBalance = 0m, DateTimeOffset? closedAt = null, CancellationToken cancellationToken = default)
        => updateAccountingPeriodHandler.Handle(new UpdateAccountingPeriodCommand(id, name, startDate, endDate, status, totalIncome, totalExpenses, netBalance, closedAt), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes an accounting period.")]
    public Task DeleteAccountingPeriod(Guid id, CancellationToken cancellationToken = default)
        => deleteAccountingPeriodHandler.Handle(new DeleteAccountingPeriodCommand(id), cancellationToken);
}
