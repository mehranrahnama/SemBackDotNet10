using Microsoft.AspNetCore.Http;

namespace SemBackDotNet10.SecurityDomain.Configuration;

/// <summary>
/// معادل HttpCookieOAuth2AuthorizationRequestRepository جاوا.
/// در Spring، این کلاس AuthorizationRequest را در کوکی ذخیره می‌کرد؛
/// در ASP.NET Core همین کار با IHttpContextAccessor انجام می‌شود.
/// </summary>
public class HttpCookieOAuth2AuthorizationRequestRepository
{
    private const int CookieExpireSeconds = 300;
    private readonly bool _isSecure;

    public HttpCookieOAuth2AuthorizationRequestRepository(Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        // معادل @Value("${sem.server.ssl.enabled}")
        _isSecure = configuration.GetValue("Sem:Server:Ssl:Enabled", false);
    }

    /// <summary>معادل loadAuthorizationRequest(request).</summary>
    public OAuth2AuthorizationRequest? LoadAuthorizationRequest(HttpRequest request)
    {
        var cookie = CookieUtils.GetCookie(request, CookieUtils.Oauth2AuthorizationRequestCookieName);
        return cookie is null ? null : CookieUtils.Deserialize<OAuth2AuthorizationRequest>(cookie);
    }

    /// <summary>معادل saveAuthorizationRequest(...): ذخیره‌ی دو کوکی (authorization request و redirectUri).</summary>
    public void SaveAuthorizationRequest(OAuth2AuthorizationRequest? authorizationRequest, HttpContext context)
    {
        if (authorizationRequest is null)
        {
            CookieUtils.DeleteCookie(context.Request, context.Response, CookieUtils.Oauth2AuthorizationRequestCookieName);
            CookieUtils.DeleteCookie(context.Request, context.Response, CookieUtils.RedirectUriParamCookieName);
            return;
        }

        CookieUtils.AddCookie(context.Response, CookieUtils.Oauth2AuthorizationRequestCookieName,
            CookieUtils.Serialize(authorizationRequest), CookieExpireSeconds, _isSecure, true);

        var redirectUriAfterLogin = context.Request.Query[CookieUtils.RedirectUriParamCookieName].ToString();

        if (!string.IsNullOrWhiteSpace(redirectUriAfterLogin))
        {
            CookieUtils.AddCookie(context.Response, CookieUtils.RedirectUriParamCookieName,
                redirectUriAfterLogin, CookieExpireSeconds, _isSecure, true);
        }
    }

    /// <summary>معادل removeAuthorizationRequest(...).</summary>
    public OAuth2AuthorizationRequest? RemoveAuthorizationRequest(HttpContext context) =>
        LoadAuthorizationRequest(context.Request);

    /// <summary>حذف هر دو کوکی.</summary>
    public void RemoveAuthorizationRequestCookies(HttpContext context)
    {
        CookieUtils.DeleteCookie(context.Request, context.Response, CookieUtils.Oauth2AuthorizationRequestCookieName);
        CookieUtils.DeleteCookie(context.Request, context.Response, CookieUtils.RedirectUriParamCookieName);
    }
}
