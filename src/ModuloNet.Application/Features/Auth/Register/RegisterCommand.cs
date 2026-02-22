using MediatR;

namespace ModuloNet.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string? DisplayName,
    string Role,
    string? ParentEmail) : IRequest<AuthTokensResponse>;

public sealed record AuthTokensResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    string Role,
    string? DisplayName);
