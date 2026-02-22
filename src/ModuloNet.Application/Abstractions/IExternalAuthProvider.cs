namespace ModuloNet.Application.Abstractions;

public interface IExternalAuthProvider
{
    string ProviderName { get; }
    Task<ExternalUserInfo?> ValidateAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}

public sealed record ExternalUserInfo(string ProviderUserId, string? Email, string? DisplayName);
