using Google.Apis.Auth.OAuth2;

namespace FinanzasMCP.McpServer.Storage;

public sealed class GoogleCredentialResolver(IConfiguration configuration)
{
    public GoogleCredential? Resolve()
    {
        try
        {
            var serviceAccountPath = configuration["Firebase:ServiceAccountPath"];
            if (!string.IsNullOrWhiteSpace(serviceAccountPath))
            {
                return GoogleCredential.FromFile(Environment.ExpandEnvironmentVariables(serviceAccountPath));
            }

            return GoogleCredential.GetApplicationDefault();
        }
        catch
        {
            return null;
        }
    }
}
