namespace SemBackDotNet10.SecurityDomain.Model.Entity;

public sealed class KeycloakOptions
{
    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; } = true;
    public int ClockSkewSeconds { get; set; } = 30;
}
