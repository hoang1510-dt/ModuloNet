using MediatR;

namespace ModuloNet.Application.Features.Auth.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest;
