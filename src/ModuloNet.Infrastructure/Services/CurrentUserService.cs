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
}
