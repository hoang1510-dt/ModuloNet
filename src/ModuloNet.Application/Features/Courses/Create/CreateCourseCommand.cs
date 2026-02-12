using MediatR;

namespace ModuloNet.Application.Features.Courses.Create;

public record CreateCourseCommand(string Title, string? Description) : IRequest<CreateCourseResult>;

public record CreateCourseResult(Guid Id);
