using System.Security.Claims;
using SemBackDotNet10.SecurityDomain.Infrastructure.Persistence;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Security;

public sealed class LocalUserClaimsMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        TenantContext tenantContext,
        IUserRepository userRepository,
        IUserAccountRepository accountRepository,
        IRoleRepository roleRepository)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        if (!tenantContext.HasTenant)
        {
            await RejectAsync(context, "TENANT_NOT_FOUND");
            return;
        }

        var subject = context.User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(subject))
        {
            await RejectAsync(context, "LOCAL_USER_MAPPING_REQUIRED");
            return;
        }

        var isLocal = context.User.FindFirstValue("iss") == SemBackDotNet10.SecurityDomain.Infrastructure.Authentication.LocalTokenService.Issuer;
        var user = isLocal && long.TryParse(subject, out var localUserId)
            ? await userRepository.FindByIdAsync(tenantContext.TenantId, localUserId, context.RequestAborted)
            : await userRepository.FindByExternalSubjectAsync(tenantContext.TenantId, subject, context.RequestAborted);
        var account = user is null
            ? null
            : isLocal
                ? await accountRepository.FindByUsernameAsync(tenantContext.TenantId, context.User.FindFirstValue("username") ?? "", context.RequestAborted)
                : await accountRepository.FindByUserIdAsync(
                tenantContext.TenantId,
                user.UserId,
                context.RequestAborted);

        if (user is null || account is null || account.UserId != user.UserId ||
            (isLocal && context.User.FindFirstValue("account_id") != account.Id.ToString()) ||
            user.Retired == true || user.LoginDisabled == true)
        {
            await RejectAsync(context, "LOCAL_USER_ACCESS_DENIED");
            return;
        }

        var roles = await roleRepository.GetForUserAsync(
            tenantContext.TenantId,
            user.UserId,
            context.RequestAborted);
        var identity = context.User.Identity as ClaimsIdentity;
        if (identity is null)
        {
            await RejectAsync(context, "INVALID_AUTHENTICATED_IDENTITY");
            return;
        }

        identity.AddClaim(new Claim("tenant_id", tenantContext.TenantId.ToString()));
        identity.AddClaim(new Claim("user_id", user.UserId.ToString()));
        identity.AddClaim(new Claim("account_id", account.Id.ToString()));
        foreach (var claim in identity.FindAll("role_id").ToArray())
            identity.RemoveClaim(claim);
        if (isLocal)
            foreach (var claim in identity.FindAll(identity.RoleClaimType).ToArray())
                identity.RemoveClaim(claim);
        foreach (var role in roles)
        {
            identity.AddClaim(new Claim(identity.RoleClaimType, role.Name));
            identity.AddClaim(new Claim("role_id", role.Id.ToString()));
        }

        await next(context);
    }

    private static async Task RejectAsync(HttpContext context, string code)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsJsonAsync(new { code, message = "Local authorization failed." });
    }
}
