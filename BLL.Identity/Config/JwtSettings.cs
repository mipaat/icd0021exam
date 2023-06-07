namespace BLL.Identity.Config;

public class JwtSettings
{
    public const string SectionKey = "JWT";

    public string Key { get; set; } = default!;

    public string Issuer { get; set; } = default!;

    public string Audience { get; set; } = default!;

    public int ExpiresInSeconds { get; set; } = 60;

    public int RefreshTokenExpiresInDays { get; set; } = 7;

    public int ExtendOldRefreshTokenExpirationByMinutes { get; set; } = 1;
}