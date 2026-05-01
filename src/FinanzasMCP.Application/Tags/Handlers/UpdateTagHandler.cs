using FinanzasMCP.Application.Common.DTOs;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Tags.Commands;
using FinanzasMCP.Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Tags.Handlers;

public sealed class UpdateTagHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task<TagSummary> Handle(UpdateTagCommand command, CancellationToken cancellationToken = default)
    {
        var tag = await dbContext.Set<Tag>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        tag.UpdateDetails(command.Name, command.Color);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new TagSummary(tag.Id, tag.Name, tag.Color);
    }
}
