using Common.exception;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using BusinessException = global::SemBackDotNet10.Common.exception.BusinessException;
using SemBackDotNet10.SecurityDomain.Configuration;
using SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;
using SemBackDotNet10.SecurityDomain.Interfaces;

namespace SemBackDotNet10.SecurityDomain.Handler;

/// <summary>OAuth2 ticket handler equivalent to the Java authentication success handler.</summary>
public sealed class OAuth2AuthenticationSuccessHandler(
    IUserAccountRepository accountRepository,
    IRefreshTokenService refreshTokenService,
    IOptions<OAuth2Options> options)
{
    private readonly OAuth2Options _options = options.Value;

    public async Task OnTicketReceivedAsync(TicketReceivedContext context)
    {
        var http = context.HttpContext;
        if (http.Response.HasStarted || context.Principal is null)
            return;

        var targetUrl = await DetermineTargetUrlAsync(http, context, context.Principal);
        AuthenticationCookieUtils.Delete(
            http.Response,
            AuthenticationCookieUtils.RedirectUriCookieName);
        http.Response.Redirect(targetUrl);
        context.HandleResponse();
    }

    private async Task<string> DetermineTargetUrlAsync(
        HttpContext http,
        TicketReceivedContext context,
        System.Security.Claims.ClaimsPrincipal principal)
    {
        var targetUrl = AuthenticationCookieUtils.Get(
            http.Request,
            AuthenticationCookieUtils.RedirectUriCookieName) ?? _options.DefaultRedirectUri;

        var accessToken = await context.HttpContext.GetTokenAsync("access_token")
            ?? principal.FindFirst("access_token")?.Value;
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new BusinessException("access token not found");

        var accessLifetime = TimeSpan.FromMinutes(Math.Max(_options.AccessTokenExpirationMinutes, 1));
        AuthenticationCookieUtils.Append(
            http.Response,
            _options.AccessTokenCookieName,
            accessToken,
            accessLifetime,
            _options.SecureCookies,
            httpOnly: false);

        var tenantId = GetLongClaim(principal, "tenant_id");
        var username = principal.Identity?.Name
            ?? principal.FindFirst("preferred_username")?.Value;
        if (!tenantId.HasValue || string.IsNullOrWhiteSpace(username))
            throw new BusinessException("authenticated tenant or username not found");

        var account = await accountRepository.FindByUsernameAsync(
            tenantId.Value,
            username,
            http.RequestAborted);
        if (account is null)
            throw new BusinessException("user account not found");

        var refresh = await refreshTokenService.CreateAsync(
            tenantId.Value,
            account.Id,
            http.RequestAborted);
        AuthenticationCookieUtils.Append(
            http.Response,
            _options.RefreshTokenCookieName,
            refresh.Token,
            refresh.ExpiresAt - DateTimeOffset.UtcNow,
            _options.SecureCookies,
            httpOnly: false);

        if (GetLongClaim(principal, "tenant_id") is { } currentOrganId)
        {
            AddOrganizationCookie(http, currentOrganId, accessLifetime);
        }

        return QueryHelpers.AddQueryString(
            targetUrl,
            "maxage",
            Math.Max(0, (refresh.ExpiresAt - DateTimeOffset.UtcNow).TotalMilliseconds).ToString("0"));
    }

    private void AddOrganizationCookie(HttpContext http, long organizationId, TimeSpan lifetime) =>
        AuthenticationCookieUtils.Append(
            http.Response,
            _options.CurrentOrganizationCookieName,
            organizationId.ToString(),
            lifetime,
            _options.SecureCookies,
            httpOnly: false);

    private static long? GetLongClaim(
        System.Security.Claims.ClaimsPrincipal principal,
        string type) => long.TryParse(principal.FindFirst(type)?.Value, out var value) ? value : null;

}
