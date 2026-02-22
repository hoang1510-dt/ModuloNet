using MediatR;
using ModuloNet.Application.Abstractions;
using ModuloNet.Application.Features.Auth;

namespace ModuloNet.Application.Features.Auth.Login;

internal sealed class LoginHandler : IRequestHandler<LoginCommand, AuthTokensResponse?>
{
    private readonly IAuthService _auth;

    public LoginHandler(IAuthService auth) => _auth = auth;

    public async Task<AuthTokensResponse?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _auth.LoginAsync(
            new LoginRequest(request.Email, request.Password),
            cancellationToken);

        return result is null ? null : ToResponse(result);
    }

    private static AuthTokensResponse ToResponse(AuthTokensResult r) =>
        new(r.AccessToken, r.RefreshToken, r.ExpiresAtUtc, r.UserId, r.Email, r.Role, r.DisplayName);
}
