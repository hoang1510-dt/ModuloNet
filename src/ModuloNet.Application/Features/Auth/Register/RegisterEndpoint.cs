using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ModuloNet.Application.Features.Auth.Register;

public static class RegisterEndpoint
{
    public static void MapRegister(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (RegisterCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("Register")
        .WithTags("Auth")
        .Produces<AuthTokensResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem();
    }
}
