using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ModuloNet.Application.Features.Auth.Login;

public static class LoginEndpoint
{
    public static void MapLogin(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (LoginCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result is null ? Results.Unauthorized() : Results.Ok(result);
        })
        .WithName("Login")
        .WithTags("Auth")
        .Produces<AuthTokensResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .ProducesValidationProblem();
    }
}
