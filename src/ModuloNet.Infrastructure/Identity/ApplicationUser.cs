using Microsoft.AspNetCore.Identity;

namespace ModuloNet.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }
}
