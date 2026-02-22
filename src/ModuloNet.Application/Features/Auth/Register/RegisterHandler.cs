using MediatR;
using ModuloNet.Application.Abstractions;
using ModuloNet.Application.Features.Auth;

namespace ModuloNet.Application.Features.Auth.Register;

internal sealed class RegisterHandler : IRequestHandler<RegisterCommand, AuthTokensResponse>
{
    private readonly IAuthService _auth;

    public RegisterHandler(IAuthService auth) => _auth = auth;

    public async Task<AuthTokensResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _auth.RegisterAsync(
            new RegisterUserRequest(
                request.Email,
                request.Password,
                request.DisplayName,
                request.Role,
                request.ParentEmail),
            cancellationToken);

        return ToResponse(result);
    }

    private static AuthTokensResponse ToResponse(AuthTokensResult r) =>
        new(r.AccessToken, r.RefreshToken, r.ExpiresAtUtc, r.UserId, r.Email, r.Role, r.DisplayName);
}
