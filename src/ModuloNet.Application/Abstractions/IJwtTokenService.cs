namespace ModuloNet.Application.Abstractions;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(Guid userId, string email, string role, string? displayName);
    (string Token, DateTime ExpiresAtUtc) GenerateRefreshToken();
}
