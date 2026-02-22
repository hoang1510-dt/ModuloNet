using MediatR;
using ModuloNet.Application.Features.Auth;

namespace ModuloNet.Application.Features.Auth.Refresh;

public sealed record RefreshCommand(string RefreshToken) : IRequest<AuthTokensResponse?>;
