using MediatR;

namespace ModuloNet.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthTokensResponse?>;

public sealed record AuthTokensResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    string Role,
    string? DisplayName);
