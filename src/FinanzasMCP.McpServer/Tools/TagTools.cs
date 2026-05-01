using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Tags.Commands;
using FinanzasMCP.Application.Tags.Handlers;
using FinanzasMCP.Application.Tags.Queries;
using ModelContextProtocol.Server;

namespace FinanzasMCP.McpServer.Tools;

[McpServerToolType]
public sealed class TagTools(
    CreateTagHandler createTagHandler,
    GetTagsHandler getTagsHandler,
    UpdateTagHandler updateTagHandler,
    DeleteTagHandler deleteTagHandler)
{
    [McpServerTool, System.ComponentModel.Description("Creates a tag.")]
    public Task<TagSummary> CreateTag(string name, string? color = null, CancellationToken cancellationToken = default)
        => createTagHandler.Handle(new CreateTagCommand(name, color), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Lists all tags.")]
    public Task<IReadOnlyList<TagSummary>> ListTags(CancellationToken cancellationToken = default)
        => getTagsHandler.Handle(new GetTagsQuery(), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Updates a tag.")]
    public Task<TagSummary> UpdateTag(Guid id, string name, string? color = null, CancellationToken cancellationToken = default)
        => updateTagHandler.Handle(new UpdateTagCommand(id, name, color), cancellationToken);

    [McpServerTool, System.ComponentModel.Description("Logically deletes a tag.")]
    public Task DeleteTag(Guid id, CancellationToken cancellationToken = default)
        => deleteTagHandler.Handle(new DeleteTagCommand(id), cancellationToken);
}
