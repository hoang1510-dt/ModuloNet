using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ModuloNet.Application.Features.Courses.Delete;

public static class DeleteCourseEndpoint
{
    public static void MapDeleteCourse(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/courses/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var deleted = await mediator.Send(new DeleteCourseCommand(id), ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteCourse")
        .WithTags("Courses")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
