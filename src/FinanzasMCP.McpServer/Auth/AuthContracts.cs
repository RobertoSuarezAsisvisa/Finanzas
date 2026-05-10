namespace FinanzasMCP.McpServer.Auth;

public sealed record RegisterRequest(string Email, string Password, string? DisplayName);
public sealed record LoginRequest(string Email, string Password);
public sealed record FirebaseLoginRequest(string IdToken);
public sealed record AuthUserResponse(Guid Id, string Email, string DisplayName);
public sealed record AuthResponse(string AccessToken, DateTimeOffset ExpiresAt, AuthUserResponse User);
