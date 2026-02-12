using MediatR;

namespace ModuloNet.Application.Features.Courses.GetById;

public record GetCourseByIdQuery(Guid Id) : IRequest<GetCourseByIdResult?>;

public record GetCourseByIdResult(Guid Id, string Title, string? Description, DateTime CreatedAtUtc);
