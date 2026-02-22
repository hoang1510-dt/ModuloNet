using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ModuloNet.Application.Features.Auth.Logout;

public static class LogoutEndpoint
{
    public static void MapLogout(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/logout", async (LogoutRequest body, IMediator mediator, CancellationToken ct) =>
        {
            await mediator.Send(new LogoutCommand(body.RefreshToken), ct);
            return Results.NoContent();
        })
        .WithName("Logout")
        .WithTags("Auth")
        .Produces(StatusCodes.Status204NoContent);
    }
}

public sealed record LogoutRequest(string RefreshToken);
