using FinanzasMCP.Application.Auth.Commands;
using FinanzasMCP.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanzasMCP.Application.Auth.Handlers;

public sealed class DeleteUserApiKeyHandler(
    IFinanzasMCPDbContext dbContext,
    ICurrentUser currentUser)
{
    public async Task Handle(DeleteUserApiKeyCommand command, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.UserId ?? throw new InvalidOperationException("An authenticated user is required.");
        var apiKey = await dbContext.UserApiKeys
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.UserId == userId, cancellationToken)
            ?? throw new InvalidOperationException("API key not found.");

        apiKey.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
