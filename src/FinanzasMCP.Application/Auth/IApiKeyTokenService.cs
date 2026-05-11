namespace FinanzasMCP.Application.Auth;

public interface IApiKeyTokenService
{
    GeneratedApiKey Create();
    bool TryParse(string value, out ParsedApiKey parsed);
    bool Verify(string secret, string secretHash);
}

public sealed record GeneratedApiKey(
    string PlainTextKey,
    string LookupKey,
    string SecretHash);

public sealed record ParsedApiKey(
    string LookupKey,
    string Secret);
