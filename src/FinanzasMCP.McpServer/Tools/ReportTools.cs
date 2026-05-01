using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Reports.Handlers;
using FinanzasMCP.Application.Reports.Queries;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class ReportTools(GetFinanceOverviewHandler getFinanceOverviewHandler)
{
    [McpServerTool, System.ComponentModel.Description("Returns a high-level financial overview.")]
    public Task<FinanceOverviewSummary> GetFinanceOverview(CancellationToken cancellationToken = default)
        => getFinanceOverviewHandler.Handle(new GetFinanceOverviewQuery(), cancellationToken);
}
