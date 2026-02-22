namespace ModuloNet.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Key { get; set; } = "ModuloNet.Default.Key.Minimum32Characters!";
    public string Issuer { get; set; } = "ModuloNet";
    public string Audience { get; set; } = "ModuloNet";
    public int AccessTokenMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 14;
}
