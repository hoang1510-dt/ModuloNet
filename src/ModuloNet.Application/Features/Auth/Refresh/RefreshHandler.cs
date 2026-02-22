using MediatR;
using ModuloNet.Application.Abstractions;

namespace ModuloNet.Application.Features.Auth.Refresh;

internal sealed class RefreshHandler : IRequestHandler<RefreshCommand, AuthTokensResponse?>
{
    private readonly IAuthService _auth;

    public RefreshHandler(IAuthService auth) => _auth = auth;

    public async Task<AuthTokensResponse?> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var result = await _auth.RefreshAsync(request.RefreshToken, cancellationToken);
        return result is null ? null : ToResponse(result);
    }

    private static AuthTokensResponse ToResponse(AuthTokensResult r) =>
        new(r.AccessToken, r.RefreshToken, r.ExpiresAtUtc, r.UserId, r.Email, r.Role, r.DisplayName);
}
