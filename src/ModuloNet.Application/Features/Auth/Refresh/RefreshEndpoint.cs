using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ModuloNet.Application.Features.Auth.Refresh;

public static class RefreshEndpoint
{
    public static void MapRefresh(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/refresh", async (RefreshRequest body, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new RefreshCommand(body.RefreshToken), ct);
            return result is null ? Results.Unauthorized() : Results.Ok(result);
        })
        .WithName("Refresh")
        .WithTags("Auth")
        .Produces<AuthTokensResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}

public sealed record RefreshRequest(string RefreshToken);
