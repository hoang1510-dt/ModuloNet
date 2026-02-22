using MediatR;
using ModuloNet.Application.Features.Auth;

namespace ModuloNet.Application.Features.Auth.ExternalExchange;

public sealed record ExternalExchangeCommand(
    string Provider,
    string AccessToken,
    string Role,
    string? ParentEmail) : IRequest<AuthTokensResponse>;
