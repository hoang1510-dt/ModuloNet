using MediatR;

namespace ModuloNet.Application.Features.Auth.Refresh;

public sealed record RefreshCommand(string RefreshToken) : IRequest<AuthTokensResponse?>;

public sealed record AuthTokensResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    string Role,
    string? DisplayName);
