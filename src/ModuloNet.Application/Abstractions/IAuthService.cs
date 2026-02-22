namespace ModuloNet.Application.Abstractions;

public interface IAuthService
{
    Task<AuthTokensResult> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    Task<AuthTokensResult?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthTokensResult?> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<AuthTokensResult> ExchangeExternalTokenAsync(ExternalTokenExchangeRequest request, CancellationToken cancellationToken = default);
    Task<AuthTokensResult> CompleteExternalLoginAsync(ExternalLoginCompletionRequest request, CancellationToken cancellationToken = default);
}

public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string? DisplayName,
    string Role,
    string? ParentEmail);

public sealed record LoginRequest(string Email, string Password);

public sealed record ExternalTokenExchangeRequest(
    string Provider,
    string AccessToken,
    string Role,
    string? ParentEmail);

public sealed record ExternalLoginCompletionRequest(
    string Provider,
    string ProviderUserId,
    string? Email,
    string? DisplayName,
    string Role,
    string? ParentEmail);

public sealed record AuthTokensResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    string Role,
    string? DisplayName);
