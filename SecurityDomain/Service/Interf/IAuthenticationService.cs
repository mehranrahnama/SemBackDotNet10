using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using SemBackDotNet10.SecurityDomain.Model.dto.jwt;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemUserDetail = global::SemBackDotNet10.SecurityDomain.Model.SemUserDetail.SemUserDetail;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

/// <summary>
/// معادل AuthenticationService.java — اینترفیس سرویس احراز هویت.
/// jakarta.servlet HttpServletRequest/HttpServletResponse → Microsoft.AspNetCore.Http.HttpContext.
/// net.minidev.json.JSONObject → System.Text.Json.Nodes.JsonObject.
/// org.springframework.security.core.Authentication → SemUserDetail (کلاس Principal سفارشی پروژه).
/// Collection&lt;? extends GrantedAuthority&gt; → ICollection&lt;string&gt; (نام نقش‌ها).
/// </summary>
public interface IAuthenticationService
{
    /// <summary>معادل loginUser(JwtRequest, HttpServletResponse).</summary>
    JwtResponse LoginUser(JwtRequest authenticationRequest, HttpContext context);

    /// <summary>معادل validateJwtTokenAndReportAccess(String, HttpServletRequest).</summary>
    bool ValidateJwtTokenAndReportAccess(string body, HttpContext context);

    /// <summary>معادل refreshToken(JwtRefreshRequest) با @Valid — اعتبارسنجی با FluentValidation یا DataAnnotations انجام می‌شود.</summary>
    JwtRefreshResponse RefreshToken(JwtRefreshRequest request);

    /// <summary>معادل getAuthorities(UserAccount, Long, String).</summary>
    ICollection<string> GetAuthorities(UserAccount account, long? curOrgan, string? originalUser);

    /// <summary>معادل getAuthoritiesOfCurOrganAndApp(UserAccount, Long).</summary>
    ICollection<string> GetAuthoritiesOfCurOrganAndApp(UserAccount account, long? curOrgan);

    /// <summary>معادل authenticate(String, String).</summary>
    SemUserDetail Authenticate(string username, string password);

    /// <summary>معادل isValid(JwtRefreshRequest).</summary>
    JsonObject IsValid(JwtRefreshRequest request);

    /// <summary>معادل logOff(HttpServletRequest, HttpServletResponse).</summary>
    void LogOff(HttpContext context);

    /// <summary>معادل synchronizeWithSSO(HttpServletRequest, HttpServletResponse, String, Map).</summary>
    void SynchronizeWithSSO(HttpContext context, string username, Dictionary<string, string> headers);

    /// <summary>معادل synchronizeUserWithSSO(HttpServletRequest, HttpServletResponse, String).</summary>
    void SynchronizeUserWithSSO(HttpContext context, string username);

    /// <summary>معادل setCookiesForSSOSignin(HttpServletRequest, HttpServletResponse, String, Map).</summary>
    void SetCookiesForSSOSignin(HttpContext context, string username, Dictionary<string, string> headers);

    /// <summary>معادل setSSOTokenToWhiteList(HttpServletRequest, HttpServletResponse, String).</summary>
    void SetSSOTokenToWhiteList(HttpContext context, string username);
}
