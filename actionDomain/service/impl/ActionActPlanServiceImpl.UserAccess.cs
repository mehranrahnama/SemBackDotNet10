using System.Linq;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public bool? userIsAdmin(long? userId)
    {
        if (userId is null) return false;
        return _userRoleService.FindUserRoles(userId.Value)
            .Any(userRole => userRole.Role != null && userRole.Role.RoleName == "Administrators");
    }
}
