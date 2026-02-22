using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModuloNet.Application.Auth;
using ModuloNet.Infrastructure.Auth;
using ModuloNet.Infrastructure.Identity;

namespace ModuloNet.Infrastructure.Persistence.Seed;

public sealed class IdentitySeeder
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly BootstrapAdminOptions _adminOptions;
    private readonly ILogger<IdentitySeeder> _logger;

    public IdentitySeeder(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOptions<BootstrapAdminOptions> adminOptions,
        ILogger<IdentitySeeder> logger)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _adminOptions = adminOptions.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedRolesAsync(cancellationToken);
        await SeedAdminUserAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        foreach (var roleName in new[] { AuthRoles.Admin, AuthRoles.Parent, AuthRoles.Student })
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                continue;
            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            _logger.LogInformation("Seeded role: {Role}", roleName);
        }
    }

    private async Task SeedAdminUserAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_adminOptions.Email))
            return;

        var existing = await _userManager.FindByEmailAsync(_adminOptions.Email);
        if (existing is not null)
            return;

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = _adminOptions.Email,
            Email = _adminOptions.Email,
            EmailConfirmed = true,
            DisplayName = _adminOptions.DisplayName ?? "System Admin",
            CreatedAtUtc = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, _adminOptions.Password);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to create bootstrap admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await _userManager.AddToRoleAsync(user, AuthRoles.Admin);
        _logger.LogInformation("Seeded bootstrap admin user: {Email}", _adminOptions.Email);
    }
}
