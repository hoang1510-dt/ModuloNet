namespace ModuloNet.Application.Abstractions;

public interface IRefreshTokenStore
{
    Task StoreAsync(Guid userId, string token, DateTime expiresAtUtc, CancellationToken cancellationToken = default);
    Task<RefreshTokenUserInfo?> ConsumeAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeAsync(string token, CancellationToken cancellationToken = default);
}

public sealed record RefreshTokenUserInfo(Guid UserId, string Email, string Role, string? DisplayName);
