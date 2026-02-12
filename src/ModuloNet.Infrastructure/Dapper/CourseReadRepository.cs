using System.Data;
using Dapper;
using ModuloNet.Application.Abstractions;
using ModuloNet.Application.Features.Courses.GetById;

namespace ModuloNet.Infrastructure.Dapper;

internal sealed class CourseReadRepository : ICourseReadRepository
{
    private readonly IDbConnection _connection;

    public CourseReadRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<GetCourseByIdResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT Id, Title, Description, CreatedAtUtc FROM Courses WHERE Id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<GetCourseByIdResult>(sql, new { Id = id });
    }
}
