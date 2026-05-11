namespace FinanzasMCP.Application.Auth;

public sealed record CreatedUserApiKey(
    string ApiKey,
    UserApiKeySummary Summary);
