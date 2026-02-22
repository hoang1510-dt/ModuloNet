using MediatR;
using ModuloNet.Application.Features.Auth;

namespace ModuloNet.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string? DisplayName,
    string Role,
    string? ParentEmail) : IRequest<AuthTokensResponse>;
