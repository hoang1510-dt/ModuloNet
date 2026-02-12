using ModuloNet.Application.Features.Courses.GetById;

namespace ModuloNet.Application.Abstractions;

public interface ICourseReadRepository
{
    Task<GetCourseByIdResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
