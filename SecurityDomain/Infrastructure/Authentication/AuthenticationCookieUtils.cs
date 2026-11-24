using Microsoft.AspNetCore.Http;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;

public static class AuthenticationCookieUtils
{
    public const string RedirectUriCookieName = "redirect_uri";
    public const string RedirectUriParamCookieName = RedirectUriCookieName;

    public static string? Get(HttpRequest request, string name) =>
        request.Cookies.TryGetValue(name, out var value) ? value : null;

    public static void Delete(HttpResponse response, string name, string path = "/") =>
        response.Cookies.Delete(name, new CookieOptions { Path = path });

    public static void Delete(HttpRequest request, HttpResponse response, string name, string path = "/") =>
        Delete(response, name, path);

    public static void Append(
        HttpResponse response,
        string name,
        string value,
        TimeSpan lifetime,
        bool secure,
        bool httpOnly = true,
        SameSiteMode sameSite = SameSiteMode.Lax) =>
        response.Cookies.Append(name, value, new CookieOptions
        {
            HttpOnly = httpOnly,
            Secure = secure,
            SameSite = sameSite,
            Path = "/",
            MaxAge = lifetime
        });
}
