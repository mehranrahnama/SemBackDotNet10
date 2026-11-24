namespace SemBackDotNet10.SecurityDomain.Model.Entity;

public sealed class OidcProviderOptions
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public string RegistrationId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string? RedirectUri { get; set; }
    public string? IssuerUri { get; set; }
    public string? AuthorizationUri { get; set; }
    public string? TokenUri { get; set; }
    public string? JwkSetUri { get; set; }
    public string? Scope { get; set; }
}
