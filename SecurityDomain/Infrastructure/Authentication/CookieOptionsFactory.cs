namespace SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;

public sealed class CookieOptionsFactory(IConfiguration configuration)
{
    public CookieOptions CreateAccessTokenCookie(TimeSpan expiration) => Create(expiration, SameSiteMode.Lax);

    public CookieOptions CreateRefreshTokenCookie(TimeSpan expiration) => Create(expiration, SameSiteMode.Strict);

    private CookieOptions Create(TimeSpan expiration, SameSiteMode sameSite) => new()
    {
        HttpOnly = true,
        Secure = !configuration.GetValue<bool>("Cookies:AllowInsecureDevelopment"),
        SameSite = sameSite,
        Expires = DateTimeOffset.UtcNow.Add(expiration),
        Path = "/"
    };
}
