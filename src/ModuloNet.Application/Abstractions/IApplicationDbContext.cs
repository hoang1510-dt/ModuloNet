using ModuloNet.Application.Features.Courses;

namespace ModuloNet.Application.Abstractions;

public interface IApplicationDbContext
{
    void AddCourse(Course course);
    Task<bool> DeleteCourseAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
