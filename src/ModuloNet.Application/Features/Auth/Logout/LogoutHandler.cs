using MediatR;
using ModuloNet.Application.Abstractions;

namespace ModuloNet.Application.Features.Auth.Logout;

internal sealed class LogoutHandler : IRequestHandler<LogoutCommand>
{
    private readonly IAuthService _auth;

    public LogoutHandler(IAuthService auth) => _auth = auth;

    public Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        => _auth.LogoutAsync(request.RefreshToken, cancellationToken);
}
