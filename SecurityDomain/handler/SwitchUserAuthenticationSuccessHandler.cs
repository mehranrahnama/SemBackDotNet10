using System.Security.Claims;
using Common.exception;
using Microsoft.Extensions.Options;
using BusinessException = global::SemBackDotNet10.Common.exception.BusinessException;
using SemBackDotNet10.SecurityDomain.Configuration;
using SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;
using SemBackDotNet10.SecurityDomain.Infrastructure.Persistence;
using SemBackDotNet10.SecurityDomain.Interfaces;

namespace SemBackDotNet10.SecurityDomain.Handler;

public sealed class SwitchUserAuthenticationSuccessHandler(
    IUserAccountRepository accountRepository,
    IRefreshTokenService refreshTokenService,
    IOptions<SwitchUserOptions> options)
{
    private readonly SwitchUserOptions _options = options.Value;

    public async Task OnAuthenticationSuccessAsync(
        HttpContext http,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken = default)
    {
        if (http.Response.HasStarted)
            return;

        AuthenticationCookieUtils.Delete(http.Request, http.Response, "sessionKey");

        var accessToken = GetAccessToken(principal);
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new BusinessException("access token not found");

        AuthenticationCookieUtils.Append(
            http.Response,
            _options.AccessTokenCookieName,
            accessToken,
            TimeSpan.FromSeconds(Math.Max(_options.CookieExpiration, 1)),
            _options.SslEnabled,
            httpOnly: false);

        var username = principal.Identity?.Name
            ?? principal.FindFirstValue("preferred_username")
            ?? throw new BusinessException("authenticated username not found");
        var account = await FindAccountAsync(username, principal, cancellationToken)
            ?? throw new BusinessException("user account not found");
        var tenantId = GetLongClaim(principal, "tenant_id")
            ?? account.Provider?.TenantId
            ?? throw new BusinessException("tenant not found");

        var refresh = await refreshTokenService.CreateAsync(
            tenantId,
            account.Id,
            cancellationToken);
        AuthenticationCookieUtils.Append(
            http.Response,
            "refreshToken",
            refresh.Token,
            refresh.ExpiresAt - DateTimeOffset.UtcNow,
            _options.SslEnabled,
            httpOnly: false);
    }

    private async Task<SecurityDomain.Model.Entity.UserAccount?> FindAccountAsync(
        string username,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var tenantId = GetLongClaim(principal, "tenant_id");
        if (tenantId.HasValue)
            return await accountRepository.FindByUsernameAsync(tenantId.Value, username, cancellationToken);

        return null;
    }

    private static string? GetAccessToken(ClaimsPrincipal principal) =>
        principal.FindFirst("access_token")?.Value;

    private static long? GetLongClaim(ClaimsPrincipal principal, string type) =>
        long.TryParse(principal.FindFirstValue(type), out var value) ? value : null;
}
