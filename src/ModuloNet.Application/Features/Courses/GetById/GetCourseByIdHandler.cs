using MediatR;
using ModuloNet.Application.Abstractions;

namespace ModuloNet.Application.Features.Courses.GetById;

internal sealed class GetCourseByIdHandler : IRequestHandler<GetCourseByIdQuery, GetCourseByIdResult?>
{
    private readonly ICourseReadRepository _readRepo;

    public GetCourseByIdHandler(ICourseReadRepository readRepo)
    {
        _readRepo = readRepo;
    }

    public Task<GetCourseByIdResult?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        => _readRepo.GetByIdAsync(request.Id, cancellationToken);
}
