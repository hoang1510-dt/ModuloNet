using Microsoft.AspNetCore.Identity;
using ModuloNet.Application.Abstractions;
using ModuloNet.Application.Auth;
using ModuloNet.Infrastructure.Identity;

namespace ModuloNet.Infrastructure.Auth;

internal sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IDateTimeProvider _dateTime;
    private readonly IEnumerable<IExternalAuthProvider> _externalProviders;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext db,
        IJwtTokenService jwt,
        IRefreshTokenStore refreshTokenStore,
        IDateTimeProvider dateTime,
        IEnumerable<IExternalAuthProvider> externalProviders)
    {
        _userManager = userManager;
        _db = db;
        _jwt = jwt;
        _refreshTokenStore = refreshTokenStore;
        _dateTime = dateTime;
        _externalProviders = externalProviders;
    }

    public async Task<AuthTokensResult> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRole(request.Role, allowAdmin: false);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = false,
            DisplayName = request.DisplayName,
            CreatedAtUtc = _dateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, request.Role);

        return IssueTokens(user, request.Role);
    }

    public async Task<AuthTokensResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? AuthRoles.Student;
        return IssueTokens(user, role);
    }

    public async Task<AuthTokensResult?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        var userInfo = await _refreshTokenStore.ConsumeAsync(refreshToken, cancellationToken);
        if (userInfo is null)
            return null;

        var user = await _userManager.FindByIdAsync(userInfo.UserId.ToString());
        if (user is null)
            return null;

        return IssueTokens(user, userInfo.Role);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;
        await _refreshTokenStore.RevokeAsync(refreshToken, cancellationToken);
    }

    public async Task<AuthTokensResult> ExchangeExternalTokenAsync(ExternalTokenExchangeRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRole(request.Role, allowAdmin: false);

        var provider = _externalProviders.FirstOrDefault(p => string.Equals(p.ProviderName, request.Provider, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Unknown provider: {request.Provider}");

        var info = await provider.ValidateAccessTokenAsync(request.AccessToken, cancellationToken)
            ?? throw new InvalidOperationException("Invalid external token");

        var user = await FindOrCreateExternalUserAsync(provider.ProviderName, info, request.Role, cancellationToken);

        if (request.Role == AuthRoles.Student && !string.IsNullOrWhiteSpace(request.ParentEmail))
        {
            var parent = await _userManager.FindByEmailAsync(request.ParentEmail);
            if (parent is not null)
            {
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? request.Role;
        return IssueTokens(user, role);
    }

    public async Task<AuthTokensResult> CompleteExternalLoginAsync(ExternalLoginCompletionRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRole(request.Role, allowAdmin: false);

        var provider = _externalProviders.FirstOrDefault(p => string.Equals(p.ProviderName, request.Provider, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Unknown provider: {request.Provider}");

        var info = new ExternalUserInfo(request.ProviderUserId, request.Email, request.DisplayName);
        var user = await FindOrCreateExternalUserAsync(provider.ProviderName, info, request.Role, cancellationToken);

        if (request.Role == AuthRoles.Student && !string.IsNullOrWhiteSpace(request.ParentEmail))
        {
            var parent = await _userManager.FindByEmailAsync(request.ParentEmail);
            if (parent is not null)
            {
             
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? request.Role;
        return IssueTokens(user, role);
    }

    private async Task<ApplicationUser> FindOrCreateExternalUserAsync(string provider, ExternalUserInfo info, string role, CancellationToken ct)
    {
        var loginInfo = new UserLoginInfo(provider, info.ProviderUserId, provider);
        var user = await _userManager.FindByLoginAsync(provider, info.ProviderUserId);
        if (user is not null)
            return user;

        var email = info.Email ?? $"{info.ProviderUserId}@{provider.ToLowerInvariant()}.local";
        user = await _userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            await _userManager.AddLoginAsync(user, loginInfo);
            return user;
        }

        user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = !string.IsNullOrEmpty(info.Email),
            DisplayName = info.DisplayName,
            CreatedAtUtc = _dateTime.UtcNow
        };
        await _userManager.CreateAsync(user);
        await _userManager.AddLoginAsync(user, loginInfo);
        await _userManager.AddToRoleAsync(user, role);
        return user;
    }

    private AuthTokensResult IssueTokens(ApplicationUser user, string role)
    {
        var (refreshToken, refreshExpiresAt) = _jwt.GenerateRefreshToken();
        var (accessToken, accessExpiresAt) = _jwt.GenerateAccessToken(user.Id, user.Email!, role, user.DisplayName);

        _refreshTokenStore.StoreAsync(user.Id, refreshToken, refreshExpiresAt).GetAwaiter().GetResult();

        return new AuthTokensResult(
            accessToken,
            refreshToken,
            accessExpiresAt,
            user.Id,
            user.Email!,
            role,
            user.DisplayName);
    }

    private static void ValidateRole(string role, bool allowAdmin)
    {
        var valid = role is AuthRoles.Parent or AuthRoles.Student || (allowAdmin && role == AuthRoles.Admin);
        if (!valid)
            throw new ArgumentException($"Invalid role for registration: {role}");
    }
}
