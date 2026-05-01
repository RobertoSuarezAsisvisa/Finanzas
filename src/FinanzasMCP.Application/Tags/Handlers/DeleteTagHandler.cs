using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Tags.Commands;
using FinanzasMCP.Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Tags.Handlers;

public sealed class DeleteTagHandler(IFinanzasMCPDbContext dbContext)
{
    public async Task Handle(DeleteTagCommand command, CancellationToken cancellationToken = default)
    {
        var tag = await dbContext.Set<Tag>().FirstAsync(x => x.Id == command.Id, cancellationToken);
        tag.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
