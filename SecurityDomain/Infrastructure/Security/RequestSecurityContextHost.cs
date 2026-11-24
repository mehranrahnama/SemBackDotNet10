using System.Security.Claims;
using Common.utils.security;
using Common.utils.userRoles;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Security;

public sealed class RequestSecurityContextHost(IHttpContextAccessor http, TenantContext tenant, SemDbContext db)
    : ISecurityContextHost, IUserRoleIdentityHost
{
    public ClaimsPrincipal? Authentication => http.HttpContext?.User.Identity?.IsAuthenticated == true
        ? http.HttpContext.User : null;

    public SecurityUser? CurrentUser
    {
        get
        {
            var principal = Authentication;
            if (principal is null) return null;
            var id = RequiredId(principal, "user_id");
            var tenantId = CurrentOrgan!.Id;
            var roles = db.Set<SemBackDotNet10.SecurityDomain.Model.Entity.UserRole>().AsNoTracking()
                .Where(role => role.UserId == id && role.User.TenantId == tenantId && role.Role.RoleName != null)
                .Select(role => role.Role.RoleName!).Distinct().ToArray();
            return new SecurityUser(id, roles);
        }
    }

    public SecurityAccount? CurrentAccount
    {
        get
        {
            var principal = Authentication;
            if (principal is null) return null;
            var accountId = RequiredId(principal, "account_id");
            var userId = RequiredId(principal, "user_id");
            var tenantId = CurrentOrgan!.Id;
            var name = db.Set<SemBackDotNet10.SecurityDomain.Model.Entity.UserAccount>().AsNoTracking()
                .Where(account => account.Id == accountId && account.UserId == userId && account.User != null && account.User.TenantId == tenantId)
                .Select(account => account.AccountName).SingleOrDefault();
            return name is null ? null : new SecurityAccount(name);
        }
    }

    public SecurityOrgan? CurrentOrgan
    {
        get
        {
            var principal = Authentication;
            if (principal is null) return null;
            var claimId = RequiredId(principal, "tenant_id");
            if (!tenant.HasTenant || tenant.TenantId != claimId)
                throw new UnauthorizedAccessException("Authenticated tenant does not match the request tenant.");
            return new SecurityOrgan(claimId);
        }
    }

    public ClaimsPrincipal? Impersonator
    {
        get
        {
            var principal = Authentication;
            if (principal is null) return null;
            var actor = (principal.Identity as ClaimsIdentity)?.Actor;
            if (actor is not null) return new ClaimsPrincipal(actor);
            if (principal.HasClaim("is_switch_user", "true"))
                throw new InvalidOperationException("The impersonating identity is missing.");
            return null;
        }
    }

    public long? GetCurrentUserId() => CurrentUser?.Id;

    private static long RequiredId(ClaimsPrincipal principal, string claim) =>
        long.TryParse(principal.FindFirstValue(claim), out var id) && id > 0
            ? id : throw new UnauthorizedAccessException($"The required claim '{claim}' is missing.");
}
