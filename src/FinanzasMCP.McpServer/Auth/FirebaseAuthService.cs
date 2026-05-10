using FirebaseAdmin.Auth;

namespace FinanzasMCP.McpServer.Auth;

public sealed class FirebaseAuthService(IConfiguration configuration)
{
    public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(configuration["Firebase:ProjectId"]))
        {
            throw new InvalidOperationException("Missing Firebase:ProjectId configuration.");
        }

        if (FirebaseAuth.DefaultInstance is null)
        {
            throw new InvalidOperationException("Firebase Admin credentials are not configured. Set GOOGLE_APPLICATION_CREDENTIALS or run gcloud auth application-default login.");
        }

        return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken, cancellationToken);
    }
}
