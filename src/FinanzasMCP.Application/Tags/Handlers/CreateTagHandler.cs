using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Tags.Commands;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Tags.Handlers;

public sealed class CreateTagHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<TagSummary> Handle(CreateTagCommand command, CancellationToken cancellationToken = default)
    {
        var tag = Tag.Create(command.Name, command.Color);
        dbContext.Set<Tag>().Add(tag);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new TagSummary(tag.Id, tag.Name, tag.Color);
    }
}
