using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ModuloNet.Application.Abstractions;

namespace ModuloNet.Infrastructure.Services;

internal sealed class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string? UserId => _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => _accessor.HttpContext?.User?.Identity?.Name;
    public bool IsAuthenticated => _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public IReadOnlyCollection<string> Roles =>
        _accessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray()
        ?? Array.Empty<string>();

    public bool IsInRole(string role) => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
}
