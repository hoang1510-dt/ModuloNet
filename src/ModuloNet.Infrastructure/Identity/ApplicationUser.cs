using Microsoft.AspNetCore.Identity;

namespace ModuloNet.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
