using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.UserContext.Commands;
using FinanzasMCP.Application.UserContext.Handlers;
using FinanzasMCP.Application.UserContext.Queries;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class UserContextTools(
    UpsertUserContextEntryHandler upsertUserContextEntryHandler,
    GetUserContextHandler getUserContextHandler,
    DeleteUserContextEntryHandler deleteUserContextEntryHandler)
{
    [McpServerTool, System.ComponentModel.Description("Creates or updates a user context entry.")]
    public Task<UserContextEntrySummary> UpsertUserContextEntry(string key, string value, CancellationToken cancellationToken = default)
        => upsertUserContextEntryHandler.Handle(new UpsertUserContextEntryCommand(key, value), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists user context entries.")]
    public Task<IReadOnlyList<UserContextEntrySummary>> ListUserContextEntries(CancellationToken cancellationToken = default)
        => getUserContextHandler.Handle(new GetUserContextQuery(), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a user context entry.")]
    public Task DeleteUserContextEntry(string key, CancellationToken cancellationToken = default)
        => deleteUserContextEntryHandler.Handle(new DeleteUserContextEntryCommand(key), cancellationToken);
}
