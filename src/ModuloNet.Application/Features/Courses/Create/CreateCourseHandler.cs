using MediatR;
using ModuloNet.Application.Abstractions;

namespace ModuloNet.Application.Features.Courses.Create;

internal sealed class CreateCourseHandler : IRequestHandler<CreateCourseCommand, CreateCourseResult>
{
    private readonly IApplicationDbContext _db;
    private readonly IDateTimeProvider _dateTime;

    public CreateCourseHandler(IApplicationDbContext db, IDateTimeProvider dateTime)
    {
        _db = db;
        _dateTime = dateTime;
    }

    public async Task<CreateCourseResult> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAtUtc = _dateTime.UtcNow
        };
        _db.AddCourse(course);
        await _db.SaveChangesAsync(cancellationToken);
        return new CreateCourseResult(course.Id);
    }
}
