using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModuloNet.Application.Abstractions;
using ModuloNet.Infrastructure.Identity;
using ModuloNet.Infrastructure.Persistence;

namespace ModuloNet.Infrastructure.Auth;

internal sealed class RefreshTokenStore : IRefreshTokenStore
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDateTimeProvider _dateTime;

    public RefreshTokenStore(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IDateTimeProvider dateTime)
    {
        _db = db;
        _userManager = userManager;
        _dateTime = dateTime;
    }

    public async Task StoreAsync(Guid userId, string token, DateTime expiresAtUtc, CancellationToken cancellationToken = default)
    {
        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            CreatedAtUtc = _dateTime.UtcNow
        });
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshTokenUserInfo?> ConsumeAsync(string token, CancellationToken cancellationToken = default)
    {
        var entity = await _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

        if (entity is null || !entity.IsActive)
            return null;

        entity.RevokedAtUtc = _dateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(entity.User);
        var role = roles.FirstOrDefault() ?? "Student";
        return new RefreshTokenUserInfo(entity.User.Id, entity.User.Email!, role, entity.User.DisplayName);
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken = default)
    {
        var entity = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
        if (entity is not null && entity.IsActive)
        {
            entity.RevokedAtUtc = _dateTime.UtcNow;
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
