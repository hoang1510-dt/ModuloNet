using System.Text.Json;
using Microsoft.Extensions.Logging;
using ModuloNet.Application.Abstractions;

namespace ModuloNet.Infrastructure.Auth.ExternalAuth;

internal sealed class GoogleAuthProvider : IExternalAuthProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GoogleAuthProvider> _logger;

    public string ProviderName => "Google";

    public GoogleAuthProvider(HttpClient httpClient, ILogger<GoogleAuthProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ExternalUserInfo?> ValidateAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"https://oauth2.googleapis.com/tokeninfo?access_token={Uri.EscapeDataString(accessToken)}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var sub = root.GetProperty("sub").GetString();
            var email = root.TryGetProperty("email", out var e) ? e.GetString() : null;
            var name = root.TryGetProperty("name", out var n) ? n.GetString() : null;

            if (string.IsNullOrEmpty(sub))
                return null;

            return new ExternalUserInfo(sub, email, name);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Google token validation failed");
            return null;
        }
    }
}
