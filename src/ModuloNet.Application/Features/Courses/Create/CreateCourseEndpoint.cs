using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ModuloNet.Application.Features.Courses.Create;

public static class CreateCourseEndpoint
{
    public static void MapCreateCourse(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/courses", async (CreateCourseCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/courses/{result.Id}", result);
        })
        .WithName("CreateCourse")
        .WithTags("Courses")
        .Produces<CreateCourseResult>(StatusCodes.Status201Created)
        .ProducesValidationProblem();
    }
}
