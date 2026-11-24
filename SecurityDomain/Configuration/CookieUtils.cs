using System.Text;
using System.Text.Json;

namespace SemBackDotNet10.SecurityDomain.Configuration;

/// <summary>معادل CookieUtils جاوا (com.irisaco.mss.sem.Common.utils.security.CookieUtils).</summary>
public static class CookieUtils
{
    public const string Oauth2AuthorizationRequestCookieName = "oauth2_auth_request";
    public const string RedirectUriParamCookieName = "redirect_uri";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>معادل getCookie(request, name).</summary>
    public static string? GetCookie(Microsoft.AspNetCore.Http.HttpRequest request, string name) =>
        request.Cookies.TryGetValue(name, out var v) ? v : null;

    /// <summary>معادل addCookie(response, name, value, maxAge, secure, httpOnly).</summary>
    public static void AddCookie(Microsoft.AspNetCore.Http.HttpResponse response, string name, string value,
        int maxAgeSeconds, bool secure, bool httpOnly)
    {
        response.Cookies.Append(name, value, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddSeconds(maxAgeSeconds),
            MaxAge = TimeSpan.FromSeconds(maxAgeSeconds),
            Secure = secure,
            HttpOnly = httpOnly,
            Path = "/",
            SameSite = SameSiteMode.Lax
        });
    }

    /// <summary>معادل deleteCookie(request, response, name).</summary>
    public static void DeleteCookie(Microsoft.AspNetCore.Http.HttpRequest request, Microsoft.AspNetCore.Http.HttpResponse response, string name)
    {
        if (!request.Cookies.ContainsKey(name)) return;
        response.Cookies.Delete(name, new CookieOptions { Path = "/" });
    }

    /// <summary>معادل serialize: Base64(JSON).</summary>
    public static string Serialize<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    /// <summary>معادل deserialize.</summary>
    public static T? Deserialize<T>(string cookieValue)
    {
        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue));
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch
        {
            return default;
        }
    }
}

/// <summary>
/// معادل OAuth2AuthorizationRequest جاوا (Spring Security) — یک POCO ساده
/// که در کوکی ذخیره می‌شود.
/// </summary>
public class OAuth2AuthorizationRequest
{
    public string RegistrationId { get; set; } = string.Empty;
    public string AuthorizationUri { get; set; } = string.Empty;
    public string TokenUri { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string ResponseType { get; set; } = "code";
    public string State { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = new();
    public Dictionary<string, string> AdditionalParameters { get; set; } = new();
}
