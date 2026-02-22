using System.Text.Json;
using Microsoft.Extensions.Logging;
using ModuloNet.Application.Abstractions;

namespace ModuloNet.Infrastructure.Auth.ExternalAuth;

internal sealed class FacebookAuthProvider : IExternalAuthProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FacebookAuthProvider> _logger;

    public string ProviderName => "Facebook";

    public FacebookAuthProvider(HttpClient httpClient, ILogger<FacebookAuthProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ExternalUserInfo?> ValidateAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"https://graph.facebook.com/me?access_token={Uri.EscapeDataString(accessToken)}&fields=id,email,name";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var id = root.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
            var email = root.TryGetProperty("email", out var e) ? e.GetString() : null;
            var name = root.TryGetProperty("name", out var n) ? n.GetString() : null;

            if (string.IsNullOrEmpty(id))
                return null;

            return new ExternalUserInfo(id, email, name);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Facebook token validation failed");
            return null;
        }
    }
}
