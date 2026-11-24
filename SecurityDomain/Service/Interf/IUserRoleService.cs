using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using System.Collections.Generic;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

public interface IUserRoleService //: BaseService<UserRole, long, UserRoleRepository>
{
    void CreateOrDelete(long userId, List<UserRoleDto> incomingRoles);
    UserRole FindByUserIdAndRoleId(long userId, long foundRoleId);
    List<UserRoleDto> FindUesrRole(long userId);
    List<string> FindCurrentUserRolesName();
    List<Role> FindRolesByUserId(long userId);
    List<UserRole> FindUserRoles(long userId);
    void AddPayeshgarUserRole(long payeshgarId);
    void AddModirUserRole(long modirId);
    void AddTashilgarUserRole(long tashilgarId);
    void AddDabirComiteRole(long userId);
    void AddRaeesComite(long userId, string tenantName);
    void AddMasterRaterUserRole(long userId);
    void AddRaterUserRole(long userId);
    void RemoveDabirComiteRole(long userId);
    void RemoveRaeesComite(long userId);
    void Create(UserRoleDto userRoleDto);
    void Edit(UserRoleDto userRoleDto);
    bool DeleteByUser(long userId);
    void ChangeTashilgar(string body);
    void ChangeModir(string body);
    List<UserRole> FindUsersByAdminRole();
    List<UserRole> FindUsersByTashilgarRole();
    List<UserRole> FindUsersBySanayeUserRole();
    List<UserRole> FindUsersBySanayeUserRoleByPlanningLevelId(long planningLevelId);
    List<UserRole> FindUsersByDabirCommittee();
    List<UserRole> FindUsersByRaessCommittee();
    List<UserRole> FindUsersByDabirCommitteeAndPlanningLevel(long planningLevelId);
    List<UserRole> FindUsersByRaessCommitteeAndPlanningLevel(long planningLevelId);
    List<UserRole> FindUsersByTaaliUserRole(long planningLevelId);
}
