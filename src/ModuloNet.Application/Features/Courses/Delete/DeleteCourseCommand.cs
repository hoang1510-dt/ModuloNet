using MediatR;

namespace ModuloNet.Application.Features.Courses.Delete;

public record DeleteCourseCommand(Guid Id) : IRequest<bool>;
