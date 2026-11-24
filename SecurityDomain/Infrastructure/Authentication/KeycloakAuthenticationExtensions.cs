using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Handler;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;

public static class KeycloakAuthenticationExtensions
{
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<KeycloakOptions>()
            .Bind(configuration.GetSection("Authentication"))
            .Validate(options => Uri.TryCreate(options.Authority, UriKind.Absolute, out _),
                "Authentication:Authority must be an absolute URI.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience),
                "Authentication:Audience is required.")
            .ValidateOnStart();

        var keycloak = configuration.GetSection("Authentication").Get<KeycloakOptions>()
            ?? throw new InvalidOperationException("Keycloak authentication configuration is missing.");
        if (string.IsNullOrWhiteSpace(keycloak.Authority) || string.IsNullOrWhiteSpace(keycloak.Audience))
            throw new InvalidOperationException("Authentication:Authority and Authentication:Audience are required.");

        var secret = configuration["sem:jwt:secret"];
        if (!string.IsNullOrWhiteSpace(secret) && Encoding.UTF8.GetByteCount(secret) < 32)
            throw new InvalidOperationException("sem:jwt:secret must contain at least 32 UTF-8 bytes.");

        var authentication = services.AddAuthentication(string.IsNullOrWhiteSpace(secret) ? JwtBearerDefaults.AuthenticationScheme : "SemBearer");
        if (!string.IsNullOrWhiteSpace(secret))
            authentication.AddPolicyScheme("SemBearer", null, options => options.ForwardDefaultSelector = context =>
            {
                var header = context.Request.Headers.Authorization.ToString();
                var token = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? header[7..].Trim() : "";
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(token) && handler.ReadJwtToken(token).Issuer == LocalTokenService.Issuer)
                        return "SemLocal";
                }
                catch (Exception exception) when (exception is ArgumentException or SecurityTokenException) { }
                return JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("SemLocal", options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, ValidIssuer = LocalTokenService.Issuer,
                    ValidateAudience = true, ValidAudience = keycloak.Audience,
                    ValidateLifetime = true, ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ClockSkew = TimeSpan.Zero, NameClaimType = "username", RoleClaimType = "role"
                };
                AuthenticationErrorHandler.Configure(options);
                options.Events.OnTokenValidated = async context =>
                {
                    var tenant = context.HttpContext.RequestServices.GetRequiredService<TenantContext>();
                    var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                    var header = context.Request.Headers.Authorization.ToString();
                    var raw = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) ? header[7..].Trim() : "";
                    var accountClaim = context.Principal?.FindFirst("account_id")?.Value;
                    var tenantClaim = context.Principal?.FindFirst("tenant_id")?.Value;
                    if (!tenant.HasTenant || !long.TryParse(tenantClaim, out var tokenTenant) || tokenTenant != tenant.TenantId ||
                        !long.TryParse(accountClaim, out var accountId) || string.IsNullOrEmpty(raw) ||
                        !await db.Set<LegacyRefreshSession>().AnyAsync(session => session.AccountId == accountId &&
                            session.AccessTokenHash == LegacyRefreshSession.Hash(raw) && session.ExpiresAt > DateTimeOffset.UtcNow,
                            context.HttpContext.RequestAborted))
                        context.Fail("The session is invalid or expired.");
                };
            });
        authentication.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = keycloak.Authority.TrimEnd('/');
                options.Audience = keycloak.Audience;
                options.RequireHttpsMetadata = keycloak.RequireHttpsMetadata;
                options.MapInboundClaims = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = keycloak.Authority.TrimEnd('/'),
                    ValidateAudience = true,
                    ValidAudience = keycloak.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromSeconds(Math.Clamp(keycloak.ClockSkewSeconds, 0, 300)),
                    NameClaimType = "preferred_username",
                    RoleClaimType = "role"
                };

                AuthenticationErrorHandler.Configure(options);
                options.Events ??= new JwtBearerEvents();
                options.Events.OnChallenge = context =>
                {
                    var handler = context.HttpContext.RequestServices
                        .GetRequiredService<UnAuthorizedHandler>();
                    return handler.OnChallengeAsync(context);
                };
                options.Events.OnTokenValidated = async context =>
                {
                    var tenantContext = context.HttpContext.RequestServices.GetRequiredService<TenantContext>();
                    var revocation = context.HttpContext.RequestServices.GetRequiredService<IRevokedTokenService>();
                    var rawToken = (context.SecurityToken as JwtSecurityToken)?.RawData;
                    if (!tenantContext.HasTenant || string.IsNullOrWhiteSpace(rawToken) ||
                        await revocation.IsRevokedAsync(rawToken, tenantContext.TenantId, context.HttpContext.RequestAborted))
                    {
                        context.Fail("The access token has been revoked or no tenant is available.");
                    }
                };
            });

        return services;
    }
}
