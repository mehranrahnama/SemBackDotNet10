namespace SemBackDotNet10.SecurityDomain.Configuration;

public sealed class OAuth2Options
{
    public string DefaultRedirectUri { get; set; } = "/";
    public string AuthorizedRedirectUris { get; set; } = "/";
    public string AccessTokenCookieName { get; set; } = "accessToken";
    public string RefreshTokenCookieName { get; set; } = "refreshToken";
    public string CurrentOrganizationCookieName { get; set; } = "currentOrgan";
    public string CurrentOrgCookieName { get; set; } = "currentOrg";
    public string SessionCookieName { get; set; } = "sessionKey";
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 14;
    public int SessionCookieExpirationMinutes { get; set; } = 15;
    public bool SecureCookies { get; set; } = true;
}

public sealed class SwitchUserOptions
{
    public string CurrentOrgCookieName { get; set; } = "currentOrg";
    public string AccessTokenCookieName { get; set; } = "accessToken";
    public bool SslEnabled { get; set; }
    public int CookieExpiration { get; set; } = 900;
}
