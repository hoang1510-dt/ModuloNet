using MediatR;
using ModuloNet.Application.Abstractions;

namespace ModuloNet.Application.Features.Auth.ExternalExchange;

internal sealed class ExternalExchangeHandler : IRequestHandler<ExternalExchangeCommand, AuthTokensResponse>
{
    private readonly IAuthService _auth;

    public ExternalExchangeHandler(IAuthService auth) => _auth = auth;

    public async Task<AuthTokensResponse> Handle(ExternalExchangeCommand request, CancellationToken cancellationToken)
    {
        var result = await _auth.ExchangeExternalTokenAsync(
            new ExternalTokenExchangeRequest(
                request.Provider,
                request.AccessToken,
                request.Role,
                request.ParentEmail),
            cancellationToken);

        return ToResponse(result);
    }

    private static AuthTokensResponse ToResponse(AuthTokensResult r) =>
        new(r.AccessToken, r.RefreshToken, r.ExpiresAtUtc, r.UserId, r.Email, r.Role, r.DisplayName);
}
