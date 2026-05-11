using System.Security.Claims;
using FinanzasMCP.Application.Auth;
using FinanzasMCP.Application.Auth.Commands;
using FinanzasMCP.Application.Auth.Handlers;
using FinanzasMCP.Application.Auth.Queries;
using FinanzasMCP.Application.Persistence;
using FinanzasMCP.Domain.Users;
using FinanzasMCP.McpServer.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace FinanzasMCP.McpServer.Api;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this RouteGroupBuilder api)
    {
        var group = api.MapGroup("/auth").AllowAnonymous();

        group.MapPost("register", async (
            RegisterRequest request,
            IFinanzasMCPDbContext dbContext,
            PasswordHasher passwordHasher,
            JwtTokenService tokenService,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(new { message = "Email and password are required." });
            }

            if (request.Password.Length < 8)
            {
                return Results.BadRequest(new { message = "Password must be at least 8 characters." });
            }

            var email = AppUser.NormalizeEmail(request.Email);
            var isFirstUser = !await dbContext.Users.AnyAsync(ct);
            if (await dbContext.Users.AnyAsync(x => x.Email == email, ct))
            {
                return Results.Conflict(new { message = "An account with this email already exists." });
            }

            var user = AppUser.Create(email, request.DisplayName ?? email, passwordHasher.Hash(request.Password));
            user.RegisterLogin();
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(ct);
            if (isFirstUser)
            {
                var legacyDataClaimer = httpContext.RequestServices.GetRequiredService<LegacyDataClaimer>();
                await legacyDataClaimer.ClaimForFirstUserAsync(user.Id, ct);
            }

            return Results.Created("/api/v1/auth/me", CreateResponse(user, tokenService));
        });

        group.MapPost("login", async (
            LoginRequest request,
            IFinanzasMCPDbContext dbContext,
            PasswordHasher passwordHasher,
            JwtTokenService tokenService,
            CancellationToken ct) =>
        {
            var email = AppUser.NormalizeEmail(request.Email);
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email, ct);
            if (user is null || !user.IsActive || user.PasswordHash is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return Results.Unauthorized();
            }

            user.RegisterLogin();
            await dbContext.SaveChangesAsync(ct);

            return Results.Ok(CreateResponse(user, tokenService));
        });

        group.MapPost("firebase", async (
            FirebaseLoginRequest request,
            IFinanzasMCPDbContext dbContext,
            FirebaseAuthService firebaseAuth,
            JwtTokenService tokenService,
            CancellationToken ct) =>
        {
            var firebaseToken = await firebaseAuth.VerifyIdTokenAsync(request.IdToken, ct);
            var email = firebaseToken.Claims.TryGetValue("email", out var emailValue)
                ? Convert.ToString(emailValue)
                : null;

            if (string.IsNullOrWhiteSpace(email))
            {
                return Results.BadRequest(new { message = "Firebase token does not include an email." });
            }

            var normalizedEmail = AppUser.NormalizeEmail(email);
            var providerUserId = firebaseToken.Uid;
            var externalLogin = await dbContext.UserExternalLogins
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Provider == "firebase" && x.ProviderUserId == providerUserId, ct);

            AppUser user;
            if (externalLogin is not null)
            {
                externalLogin.RegisterLogin();
                user = externalLogin.User;
            }
            else
            {
                user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail, ct)
                    ?? AppUser.Create(normalizedEmail, GetDisplayName(firebaseToken.Claims, normalizedEmail));

                if (dbContext.Entry(user).State == EntityState.Detached)
                {
                    dbContext.Users.Add(user);
                    await dbContext.SaveChangesAsync(ct);
                }

                dbContext.UserExternalLogins.Add(UserExternalLogin.Create(user.Id, "firebase", providerUserId, normalizedEmail));
            }

            user.RegisterLogin();
            await dbContext.SaveChangesAsync(ct);

            return Results.Ok(CreateResponse(user, tokenService));
        });

        group.MapGet("me", (ClaimsPrincipal principal) =>
        {
            var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var name = principal.FindFirstValue(ClaimTypes.Name);
            return Guid.TryParse(id, out var userId) && email is not null
                ? Results.Ok(new AuthUserResponse(userId, email, name ?? email))
                : Results.Unauthorized();
        }).RequireAuthorization();

        var apiKeys = group.MapGroup("api-keys").RequireAuthorization();

        apiKeys.MapGet("", async ([FromServices] GetUserApiKeysHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetUserApiKeysQuery(), ct)));

        apiKeys.MapPost("", async (CreateApiKeyRequest request, [FromServices] CreateUserApiKeyHandler handler, CancellationToken ct) =>
        {
            var created = await handler.Handle(new CreateUserApiKeyCommand(request.Name), ct);
            return Results.Created($"/api/v1/auth/api-keys/{created.Summary.Id}", created);
        });

        apiKeys.MapPost("{id:guid}/revoke", async (Guid id, [FromServices] RevokeUserApiKeyHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new RevokeUserApiKeyCommand(id), ct);
            return Results.NoContent();
        });

        apiKeys.MapDelete("{id:guid}", async (Guid id, [FromServices] DeleteUserApiKeyHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteUserApiKeyCommand(id), ct);
            return Results.NoContent();
        });
    }

    private static AuthResponse CreateResponse(AppUser user, JwtTokenService tokenService)
    {
        var token = tokenService.CreateToken(user);
        return new AuthResponse(token.AccessToken, token.ExpiresAt, new AuthUserResponse(user.Id, user.Email, user.DisplayName));
    }

    private static string GetDisplayName(IReadOnlyDictionary<string, object> claims, string email)
    {
        return claims.TryGetValue("name", out var name) && !string.IsNullOrWhiteSpace(Convert.ToString(name))
            ? Convert.ToString(name)!
            : email;
    }
}
