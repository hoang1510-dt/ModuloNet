using MediatR;

namespace ModuloNet.Application.Features.Auth.ExternalExchange;

public sealed record ExternalExchangeCommand(
    string Provider,
    string AccessToken,
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
