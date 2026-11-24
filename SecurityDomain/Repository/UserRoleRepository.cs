using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Repository;

public interface IUserRoleRepository
{
    HashSet<RoleDto> FindRoleDtoByUserId(long userId);

    void RemoveUserRolesById(long userId, HashSet<long> roleIds);

    UserRole? FindByUserIdAndRoleId(long userId, long roleId);

    List<string> FindRolesNameByUserId(long userId);

    List<Role> FindRolesByUserId(long userId);

    List<UserRoleDto> FindAllUserRolesAndAllRoles(long userId);

    List<UserRole> FindUserRoles(long userId);

    List<UserRole> FindAdminUserRole();

    List<UserRole> FindUserRolesByRoleName(string roleName);

    List<UserRole> FindTashilgarUserRoles();

    List<UserRole> FindSanayeUserRoles();

    List<UserRole> FindSanayeUserRolesByPlanningLevelId(long planningLevelId);

    List<UserRole> FindDabirComiteUserRoles();

    List<UserRole> FindRaeesComiteUserRoles();

    UserRole Save(UserRole entity);

    void Delete(UserRole entity);
}

public class UserRoleRepository : SecurityRepositoryBase<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context) { }

    public HashSet<RoleDto> FindRoleDtoByUserId(long userId) =>
        Set.Where(ur => ur.User.UserId == userId)
            .Select(ur => new RoleDto { Id = ur.Role.Id, RoleName = ur.Role.RoleName, RoleDes = ur.Role.RoleDes })
            .ToHashSet();

    public void RemoveUserRolesById(long userId, HashSet<long> roleIds)
    {
        var items = Set.Where(ur => ur.User.UserId == userId && roleIds.Contains(ur.Role.Id)).ToList();
        Set.RemoveRange(items);
        Context.SaveChanges();
    }

    public UserRole? FindByUserIdAndRoleId(long userId, long roleId) =>
        Set.FirstOrDefault(ur => ur.User.UserId == userId && ur.Role.Id == roleId);

    public List<string> FindRolesNameByUserId(long userId) =>
        Set.Where(ur => ur.User.UserId == userId).Select(ur => ur.Role.RoleName).ToList();

    public List<Role> FindRolesByUserId(long userId) =>
        Set.Where(ur => ur.User.UserId == userId).Select(ur => ur.Role).ToList();

    public List<UserRoleDto> FindAllUserRolesAndAllRoles(long userId) =>
        Context.Set<Role>()
            .Select(r => new UserRoleDto
            {
                Id = r.Id,
                RoleName = r.RoleName,
                RoleDes = r.RoleDes,
                Check = Set.Any(ur => ur.Role.Id == r.Id && ur.User.UserId == userId)
            }).ToList();

    public List<UserRole> FindUserRoles(long userId) =>
        Set.Include(ur => ur.Role).Where(ur => ur.User.UserId == userId).ToList();

    public List<UserRole> FindAdminUserRole() => FindUserRolesByRoleName("Administrators");

    public List<UserRole> FindUserRolesByRoleName(string roleName) =>
        Set.Include(ur => ur.User).Where(ur => ur.Role.RoleName == roleName).ToList();

    public List<UserRole> FindTashilgarUserRoles() => FindUserRolesByRoleName("Tashilgar");

    public List<UserRole> FindSanayeUserRoles() => FindUserRolesByRoleName("SanayeUser");

    public List<UserRole> FindSanayeUserRolesByPlanningLevelId(long planningLevelId) =>
        FindRoleMembersAtPlanningLevel("SanayeUser", planningLevelId);

    public List<UserRole> FindDabirCommitteeUserRolesAndPlanningLevel(long planningLevelId) =>
        FindRoleMembersAtPlanningLevel("DabirComite", planningLevelId);

    public List<UserRole> FindRaeesCommitteeUserRolesAndPlanningLevel(long planningLevelId) =>
        FindRoleMembersAtPlanningLevel("RaeesComite", planningLevelId);

    public List<UserRole> FindTaaliUserRolesByPlanningLevel(long planningLevelId) =>
        FindRoleMembersAtPlanningLevel("TaaliUser", planningLevelId);

    private List<UserRole> FindRoleMembersAtPlanningLevel(string roleName, long planningLevelId)
    {
        // The planning entity is mapped in SemDbContext, not AppDbContext.
        // Keep the membership filter in the same SQL query as the role filter.
        var memberIds = Context.Database.SqlQuery<long>($"""
            SELECT [userid] AS [Value]
            FROM [dbo].[tblplaningleveluser]
            WHERE [levelid] = {planningLevelId} AND [userid] IS NOT NULL
            """);

        return Set.Include(ur => ur.User)
            .Where(ur => ur.Role.RoleName == roleName && memberIds.Contains(ur.User.UserId))
            .ToList();
    }

    public List<UserRole> FindDabirComiteUserRoles() => FindUserRolesByRoleName("DabirComite");

    public List<UserRole> FindRaeesComiteUserRoles() => FindUserRolesByRoleName("RaeesComite");
}
