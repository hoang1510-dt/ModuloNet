using MediatR;
using ModuloNet.Application.Abstractions;

namespace ModuloNet.Application.Features.Courses.Delete;

internal sealed class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, bool>
{
    private readonly IApplicationDbContext _db;

    public DeleteCourseHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var removed = await _db.DeleteCourseAsync(request.Id, cancellationToken);
        if (removed)
            await _db.SaveChangesAsync(cancellationToken);
        return removed;
    }
}
