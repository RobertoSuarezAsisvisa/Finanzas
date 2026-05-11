using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Application.Auth.Commands;
using FinanzasMCP.Domain.Users;

namespace FinanzasMCP.Application.Auth.Handlers;

public sealed class CreateUserApiKeyHandler(
    IFinanzasMCPDbContext dbContext,
    ICurrentUser currentUser,
    IApiKeyTokenService apiKeyTokenService)
{
    public async Task<CreatedUserApiKey> Handle(CreateUserApiKeyCommand command, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.UserId ?? throw new InvalidOperationException("An authenticated user is required.");
        var generated = apiKeyTokenService.Create();
        var apiKey = UserApiKey.Create(userId, command.Name, generated.LookupKey, generated.SecretHash);

        dbContext.UserApiKeys.Add(apiKey);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreatedUserApiKey(
            generated.PlainTextKey,
            ToSummary(apiKey));
    }

    private static UserApiKeySummary ToSummary(UserApiKey apiKey)
        => new(
            apiKey.Id,
            apiKey.Name,
            BuildPreview(apiKey.LookupKey),
            apiKey.CreatedAt,
            apiKey.LastUsedAt,
            apiKey.RevokedAt,
            apiKey.IsRevoked);

    private static string BuildPreview(string lookupKey)
        => $"fmcp_{lookupKey}_••••••••";
}
