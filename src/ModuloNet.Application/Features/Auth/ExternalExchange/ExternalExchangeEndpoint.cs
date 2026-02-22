using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ModuloNet.Application.Features.Auth;

namespace ModuloNet.Application.Features.Auth.ExternalExchange;

public static class ExternalExchangeEndpoint
{
    public static void MapExternalExchange(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/external/exchange", async (ExternalExchangeRequest body, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new ExternalExchangeCommand(body.Provider, body.AccessToken, body.Role, body.ParentEmail),
                ct);
            return Results.Ok(result);
        })
        .WithName("ExternalExchange")
        .WithTags("Auth")
        .Produces<AuthTokensResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}

public sealed record ExternalExchangeRequest(string Provider, string AccessToken, string Role, string? ParentEmail);
