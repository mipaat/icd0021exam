using Microsoft.Extensions.Configuration;

namespace BLL.Identity.Config;

public class IdentityAppDataInitSettings
{
    public const string SectionKey = "IdentityInit";

    public bool SeedIdentity { get; set; } = true;
    public bool SeedDemoIdentity { get; set; }
    public string? AdminPassword { get; set; }
}

public static class ConfigurationExtensions {
    public static IdentityAppDataInitSettings GetIdentityAppDataInitSettings(this IConfiguration configuration)
    {
        return configuration.GetSection(IdentityAppDataInitSettings.SectionKey).Get<IdentityAppDataInitSettings>() ?? new IdentityAppDataInitSettings();
    }
}