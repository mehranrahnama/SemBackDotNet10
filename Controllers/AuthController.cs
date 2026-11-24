using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.SecurityDomain.Authentication;
using SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.dto.jwt;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(IAuthenticationService authentication, LocalTokenService tokens, TenantContext tenant, IConfiguration configuration) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] JwtRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(configuration["sem:jwt:secret"]) || string.IsNullOrWhiteSpace(configuration["Authentication:Audience"]) || string.IsNullOrWhiteSpace(configuration["Authentication:Authority"]))
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "AUTHENTICATION_NOT_CONFIGURED" });
        if (!tenant.HasTenant || (request.CurrentTenant.HasValue && request.CurrentTenant != tenant.TenantId))
            return StatusCode(StatusCodes.Status403Forbidden, new { error = "TENANT_NOT_FOUND", message = "Tenant could not be resolved." });
        var result = await authentication.AuthenticateAsync(new AuthenticationRequest(request.Username, request.Password, tenant.TenantId), cancellationToken);
        if (!result.IsAuthenticated || result.UserId is null || result.AccountId is null)
            return Unauthorized(new { error = "INVALID_CREDENTIALS", message = "Invalid username or password." });
        var issued = await tokens.CreateAsync(result, cancellationToken);
        SetSessionCookie(issued);
        return Ok(new JwtResponse(issued.RefreshMilliseconds, tenant.TenantId, issued.AccessToken, issued.RefreshToken));
    }

    [AllowAnonymous]
    [HttpPost("refreshtoken")]
    public async Task<IActionResult> Refresh([FromBody] JwtRefreshRequest request, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant || (request.CurOrgan.HasValue && request.CurOrgan != tenant.TenantId))
            return Unauthorized();
        var issued = await tokens.RotateAsync(request.RefreshToken, tenant.TenantId, cancellationToken);
        if (issued is null) return Unauthorized(new { error = "INVALID_REFRESH_TOKEN" });
        SetSessionCookie(issued);
        return Ok(new JwtRefreshResponse(new SemBackDotNet10.SecurityDomain.Model.SemUserDetail.Token(
            issued.AccessToken, issued.AccessMilliseconds, issued.AccessExpiresAt), issued.RefreshToken));
    }

    [Authorize]
    [HttpGet("logoff")]
    public async Task<IActionResult> Logoff(CancellationToken cancellationToken)
    {
        var header = Request.Headers.Authorization.ToString();
        if (long.TryParse(User.FindFirstValue("account_id"), out var accountId) && header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            await tokens.RevokeAsync(header[7..].Trim(), accountId, cancellationToken);
        Response.Cookies.Delete("sessionKey", new CookieOptions { Path = Request.PathBase.HasValue ? Request.PathBase.Value : "/" });
        return Ok();
    }

    private void SetSessionCookie(IssuedSession issued)
    {
        var value = Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes($"{issued.JwtId}:{issued.Subject}")));
        Response.Cookies.Append("sessionKey", value, new CookieOptions
        {
            HttpOnly = true, Secure = Request.IsHttps, SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromMilliseconds(issued.RefreshMilliseconds), Path = Request.PathBase.HasValue ? Request.PathBase.Value : "/"
        });
    }
}
