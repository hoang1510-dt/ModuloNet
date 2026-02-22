using MediatR;
using ModuloNet.Application.Features.Auth;

namespace ModuloNet.Application.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthTokensResponse?>;
