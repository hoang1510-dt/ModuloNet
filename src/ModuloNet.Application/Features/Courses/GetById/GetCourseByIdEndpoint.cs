using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ModuloNet.Application.Features.Courses.GetById;

public static class GetCourseByIdEndpoint
{
    public static void MapGetCourseById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/courses/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetCourseByIdQuery(id), ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetCourseById")
        .WithTags("Courses")
        .Produces<GetCourseByIdResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
