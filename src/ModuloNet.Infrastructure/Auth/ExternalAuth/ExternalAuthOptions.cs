namespace ModuloNet.Infrastructure.Auth.ExternalAuth;

public sealed class ExternalAuthOptions
{
    public const string SectionName = "ExternalAuth";

    public GoogleOptions Google { get; set; } = new();
    public FacebookOptions Facebook { get; set; } = new();
}

public sealed class GoogleOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CallbackPath { get; set; } = "/api/auth/external/google/callback";
}

public sealed class FacebookOptions
{
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string CallbackPath { get; set; } = "/api/auth/external/facebook/callback";
}
