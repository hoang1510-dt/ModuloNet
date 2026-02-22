using System.Security.Claims;
using ModuloNet.Application.Abstractions;
using ModuloNet.Application.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace ModuloNet.Api.Auth;

public static class ExternalAuthEndpoints
{
    private const string RoleKey = "role";
    private const string ParentEmailKey = "parentEmail";

    public static void MapExternalAuth(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/external/{provider}/start", async (
            string provider,
            HttpContext context,
            string? role,
            string? parentEmail) =>
        {
            role ??= AuthRoles.Student;
            if (role != AuthRoles.Parent && role != AuthRoles.Student)
                return Results.BadRequest("Role must be Parent or Student.");

            var scheme = provider.ToLowerInvariant() switch
            {
                "google" => GoogleDefaults.AuthenticationScheme,
                "facebook" => FacebookDefaults.AuthenticationScheme,
                _ => (string?)null
            };

            if (scheme is null)
                return Results.BadRequest("Provider must be Google or Facebook.");

            var props = new AuthenticationProperties
            {
                RedirectUri = $"/api/auth/external/{provider}/callback",
                Items = { [RoleKey] = role, [ParentEmailKey] = parentEmail ?? string.Empty }
            };

            await context.ChallengeAsync(scheme, props);
            return Results.Empty;
        })
        .WithName("ExternalAuthStart")
        .WithTags("Auth");

        app.MapGet("/api/auth/external/{provider}/callback", async (
            string provider,
            HttpContext context,
            IAuthService auth,
            IOptions<ExternalAuthRedirectOptions> options) =>
        {
            var result = await context.AuthenticateAsync("External");

            if (!result.Succeeded || result.Principal is null)
                return Results.Redirect(options.Value.FailureUrl ?? "/login?error=external_failed");

            var providerUserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? result.Principal.FindFirstValue("sub");
            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var displayName = result.Principal.FindFirstValue(ClaimTypes.Name);
            var items = result.Properties?.Items;
            var role = (items?.ContainsKey(RoleKey) == true ? items[RoleKey] : null) ?? AuthRoles.Student;
            var parentEmail = items?.ContainsKey(ParentEmailKey) == true ? items[ParentEmailKey] : null;

            if (string.IsNullOrEmpty(providerUserId))
                return Results.Redirect(options.Value.FailureUrl ?? "/login?error=no_provider_id");

            var scheme = provider.ToLowerInvariant();
            var authResult = await auth.CompleteExternalLoginAsync(new ExternalLoginCompletionRequest(
                scheme == "google" ? "Google" : "Facebook",
                providerUserId,
                email,
                displayName,
                role,
                string.IsNullOrEmpty(parentEmail) ? null : parentEmail));

            await context.SignOutAsync("External");

            var redirectUrl = options.Value.SuccessUrl ?? "/";
            var sep = redirectUrl.Contains('?') ? "&" : "?";
            var q = $"access_token={Uri.EscapeDataString(authResult.AccessToken)}&refresh_token={Uri.EscapeDataString(authResult.RefreshToken)}&expires_at={Uri.EscapeDataString(authResult.ExpiresAtUtc.ToString("O"))}";
            return Results.Redirect($"{redirectUrl}{sep}{q}");
        })
        .WithName("ExternalAuthCallback")
        .WithTags("Auth")
        .ExcludeFromDescription();
    }
}

public sealed class ExternalAuthRedirectOptions
{
    public string? SuccessUrl { get; set; }
    public string? FailureUrl { get; set; }
}
