using SecurityUtils = Common.utils.security.SecurityUtils;
namespace Common.utils.userRoles;
public interface IUserRoleIdentityHost { long? GetCurrentUserId(); }
public sealed class UserRolesHelpers(SecurityUtils security, IUserRoleIdentityHost identity)
{
    public ISet<string> GetUserRoleNames()=>new HashSet<string>((security.GetCurrentUser() ?? throw new InvalidOperationException("Current user is not available")).RoleNames, StringComparer.Ordinal);
    public long? GetUserIdIfIsFacilitator(string tenantName)
    {
        ArgumentNullException.ThrowIfNull(tenantName);
        var roles=GetUserRoleNames();
        return roles.Count>0 && roles.All(r=>r==UserRolesConstants.FACILITATOR_ROLE_NAME) && tenantName==TenantNameConstants.TENANT_HOSCO ? identity.GetCurrentUserId() : null;
    }
    public long? GetCurrentUserId()=>(security.GetCurrentUser() ?? throw new InvalidOperationException("Current user is not available")).Id;
    public bool IsCurrentUserAdmin()=>GetUserRoleNames().Contains(UserRolesConstants.ADMIN_ROLE_NAME);
}
