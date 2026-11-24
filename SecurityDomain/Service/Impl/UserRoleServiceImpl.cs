using EntityManager = global::SemBackDotNet10.Data.SemDbContext;
using ActionPlanCatalogService = global::SemBackDotNet10.actionDomain.service.interf.IActionPlanCatalogService;
using ActionPlanService = global::SemBackDotNet10.actionDomain.service.interf.IActionPlanService;
using MeasureService = global::SemBackDotNet10.measureDomain.service.interf.IMeasureService;
using MeasureCatalogService = global::SemBackDotNet10.measureDomain.service.interf.IMeasureCatalogService;
using PlaningLevelUserService = global::SemBackDotNet10.service.impl.PlaningLevelUserServiceImpl;
using global::SemBackDotNet10.SecurityDomain.Repository;
using SemLogger = global::Common.logging.SemLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Common.exception;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Service.Interf;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

public class UserRoleServiceImpl : IUserRoleService
{
    private readonly UserRoleRepository userRoleRepository;
    private readonly IRoleService roleService;
    private readonly global::SemBackDotNet10.basicDomain.service.IUserService userService;
    private readonly EntityManager entityManager;
    private readonly global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderRepository evaluationTeamHeaderRepository;
    private readonly global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamRepository evaluationTeamRepository;
    private readonly ActionPlanCatalogService actionPlanCatalogService;

    private MeasureService measureService;
    private ActionPlanService actionPlanService;
    private PlaningLevelUserService planingLevelUserService;
    private MeasureCatalogService measureCatalogService;
    private SemLogger semLogger;
    private string tenantName;

    public UserRoleServiceImpl(
        UserRoleRepository userRoleRepository,
        IRoleService roleService,
        global::SemBackDotNet10.basicDomain.service.IUserService userService,
        EntityManager entityManager,
        global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderRepository evaluationTeamHeaderRepository,
        global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamRepository evaluationTeamRepository,
        ActionPlanCatalogService actionPlanCatalogService,
        MeasureService measureService,
        ActionPlanService actionPlanService,
        PlaningLevelUserService planingLevelUserService,
        MeasureCatalogService measureCatalogService,
        SemLogger semLogger,
        string tenantName)
    {
        this.userRoleRepository = userRoleRepository;
        this.roleService = roleService;
        this.userService = userService;
        this.entityManager = entityManager;
        this.evaluationTeamHeaderRepository = evaluationTeamHeaderRepository;
        this.evaluationTeamRepository = evaluationTeamRepository;
        this.actionPlanCatalogService = actionPlanCatalogService;
        this.measureService = measureService;
        this.actionPlanService = actionPlanService;
        this.planingLevelUserService = planingLevelUserService;
        this.measureCatalogService = measureCatalogService;
        this.semLogger = semLogger;
        this.tenantName = tenantName;
    }

    public UserRoleRepository GetRepository() => userRoleRepository;

    private void Save(UserRole role) => userRoleRepository.Save(role);

    private void SaveRoleForUser(long userId, Role? role)
    {
        var user = userService.FindByIdOrElseThrow(userId);
        ArgumentNullException.ThrowIfNull(role);
        Save(new UserRole { UserId = user.UserId, RoleId = role.Id });
    }

    private void Delete(UserRole role) => userRoleRepository.Delete(role);

    public void CreateOrDelete(long userId, List<UserRoleDto> incomingRoles)
    {
        var selectedRoleIds = ExtractSelectedRoleIds(incomingRoles);
        var currentRoles = userRoleRepository.FindRolesByUserId(userId);

        if (currentRoles.Count == 0)
            AddRolesToUser(userId, selectedRoleIds);
        else
            HandleRoleUpdates(userId, incomingRoles, selectedRoleIds, currentRoles);

        if (incomingRoles.Count != 0)
            userService.DeactivateUser(userId);

        semLogger.Info("Exit method");
    }

    private HashSet<long> ExtractSelectedRoleIds(List<UserRoleDto> roles) =>
        roles.Where(role => role.Check == true)
             .Select(role => role.Id!.Value)
             .ToHashSet();

    private void AddRolesToUser(long userId, HashSet<long> roleIds) =>
        userRoleRepository.SaveAll(MapToUserRoles(userId, roleIds));

    private void HandleRoleUpdates(long userId, List<UserRoleDto> incomingRoles,
        HashSet<long> selectedRoleIds, List<Role> currentRoles)
    {
        var currentRoleIds = ExtractRoleIds(currentRoles);
        var rolesToRemove = CalculateRolesToRemove(currentRoleIds, selectedRoleIds);
        var rolesToAdd = CalculateRolesToAdd(selectedRoleIds, currentRoleIds);

        // ValidateRoleRemoval(userId, incomingRoles, rolesToRemove);

        if (rolesToRemove.Count != 0)
            userRoleRepository.RemoveUserRolesById(userId, rolesToRemove);

        if (rolesToAdd.Count != 0)
            userRoleRepository.SaveAll(MapToUserRoles(userId, rolesToAdd));
    }

    private HashSet<long> ExtractRoleIds(List<Role> roles) =>
        roles.Select(role => role.Id).ToHashSet();

    private HashSet<long> CalculateRolesToRemove(HashSet<long> currentRoleIds, HashSet<long> selectedRoleIds)
    {
        var result = new HashSet<long>(currentRoleIds);
        result.ExceptWith(selectedRoleIds);
        return result;
    }

    private HashSet<long> CalculateRolesToAdd(HashSet<long> selectedRoleIds, HashSet<long> currentRoleIds)
    {
        var result = new HashSet<long>(selectedRoleIds);
        result.ExceptWith(currentRoleIds);
        return result;
    }

    private List<UserRole> MapToUserRoles(long userId, HashSet<long> roleIds) =>
        roleIds.Select(roleId => new UserRole { RoleId = roleId, UserId = userId }).ToList();

    private void ValidateRoleRemoval(long userId, List<UserRoleDto> incomingRoles, HashSet<long> rolesToRemove)
    {
        var masterRaterId = FindRoleIdByName(incomingRoles, "MasterRater");
        var raterId = FindRoleIdByName(incomingRoles, "Rater");

        if (masterRaterId.HasValue && rolesToRemove.Contains(masterRaterId.Value) &&
            evaluationTeamHeaderRepository.countByMasterRaterId(userId) > 0)
            throw new BusinessException("بعلت وجود تیم ارزیابی برای کاربر مورد نظر، امکان حذف نقش سرارزیاب وجود ندارد.");

        if (raterId.HasValue && rolesToRemove.Contains(raterId.Value) &&
            evaluationTeamRepository.countByRaterId(userId) > 0)
            throw new BusinessException("بعلت وجود تیم ارزیابی برای کاربر مورد نظر، امکان حذف نقش ارزیاب وجود ندارد.");
    }

    private long? FindRoleIdByName(List<UserRoleDto> roles, string roleName) =>
        roles.Where(role => Equals(role.RoleName, roleName))
             .Select(role => role.Id).FirstOrDefault();

    public List<string> FindCurrentUserRolesName()
    {
        semLogger.Info("Enter the method");
        long userId = SecurityUtils.GetCurrentUserId() ?? throw new global::SemBackDotNet10.Common.exception.UnAuthorizedException();
        var result = GetRepository().FindRolesNameByUserId(userId);
        semLogger.Info("Exit method");
        return result;
    }

    public List<Role> FindRolesByUserId(long userId) => GetRepository().FindRolesByUserId(userId);

    public List<UserRole> FindUserRoles(long userId) => GetRepository().FindUserRoles(userId);

    public UserRole FindByUserIdAndRoleId(long userId, long roleId)
    {
        semLogger.Info("Enter the method");
        var result = GetRepository().FindByUserIdAndRoleId(userId, roleId);
        semLogger.Info("Exit method");
        return result;
    }

    public List<UserRoleDto> FindUesrRole(long userId)
    {
        semLogger.Info("Enter the method");
        var userRoles = GetRepository().FindAllUserRolesAndAllRoles(userId);
        semLogger.Info("Exit method");
        return userRoles;
    }

    public void AddPayeshgarUserRole(long payeshgarId)
    {
        semLogger.Info("Enter the method");
        try
        {
            var userRoles = FindRolesByUserId(payeshgarId);
            var tashilgarRole = roleService.FindByRoleName("Tashilgar");
            if (!userRoles.Contains(tashilgarRole))
            {
                var user = userService.FindByIdOrElseThrow(payeshgarId);
                Save(new UserRole { UserId = user.UserId, RoleId = tashilgarRole!.Id });
            }
        }
        catch (Exception e)
        {
            semLogger.Error("throw exception: " + e.Message);
            Console.Error.WriteLine(e);
        }
        semLogger.Info("Exit method");
    }

    public void AddModirUserRole(long modirId)
    {
        semLogger.Info("Enter the method");
        try
        {
            var userRoles = FindRolesByUserId(modirId);
            var modirRole = roleService.FindByRoleName("Modiriat");
            if (!userRoles.Contains(modirRole))
            {
                var user = userService.FindByIdOrElseThrow(modirId);
                Save(new UserRole { UserId = user.UserId, RoleId = modirRole!.Id });
            }
        }
        catch (Exception e)
        {
            semLogger.Error("throw exception: " + e.Message);
            throw new BusinessException("خطا در تعیین نقش مدیر برای کاربر ");
        }
        semLogger.Info("Exit method");
    }

    public void AddTashilgarUserRole(long tashilgarId)
    {
        semLogger.Info("Enter the method");
        try
        {
            var userRoles = FindRolesByUserId(tashilgarId);
            var tashilgarRole = roleService.FindByRoleName("Tashilgar");
            if (!userRoles.Contains(tashilgarRole))
            {
                var user = userService.FindByIdOrElseThrow(tashilgarId);
                Save(new UserRole { UserId = user.UserId, RoleId = tashilgarRole!.Id });
            }
        }
        catch (Exception e)
        {
            semLogger.Error("throw exception: " + e.Message);
            throw new BusinessException("خطا در تعیین نقش تسهیلگر برای کاربر ");
        }
        semLogger.Info("Exit method");
    }

    public void AddDabirComiteRole(long userId)
    {
        semLogger.Info("Enter the method");
        try
        {
            var userRoles = FindRolesByUserId(userId);
            var role = roleService.FindByRoleName("DabirComite");
            if (!userRoles.Contains(role))
                SaveRoleForUser(userId, role);
        }
        catch (Exception e)
        {
            semLogger.Error("throw exception: " + e.Message);
            throw new BusinessException("خطا در تعیین نقش دبیر کمیته برای کاربر ");
        }
        semLogger.Info("Exit method");
    }

    public void AddRaeesComite(long userId, string tenantName)
    {
        semLogger.Info("Enter the method");
        try
        {
            var userRoles = FindRolesByUserId(userId);
            var role = roleService.FindByRoleName(tenantName.Equals("IRISA") ? "ModirCommitte" : "RaeesComite");
            if (!userRoles.Contains(role))
                SaveRoleForUser(userId, role);
        }
        catch (Exception e)
        {
            semLogger.Error("throw exception: " + e.Message);
            throw new BusinessException("خطا در تعیین نقش رییس کمیته برای کاربر ");
        }
        semLogger.Info("Exit method");
    }

    public void AddMasterRaterUserRole(long userId)
    {
        semLogger.Info("Enter the method");
        try
        {
            var userRoles = FindRolesByUserId(userId);
            var role = roleService.FindByRoleName("MasterRater");
            if (!userRoles.Contains(role))
                SaveRoleForUser(userId, role);
        }
        catch (Exception e)
        {
            semLogger.Error("throw exception: " + e.Message);
            Console.Error.WriteLine(e);
        }
        semLogger.Info("Exit method");
    }

    public void AddRaterUserRole(long userId)
    {
        semLogger.Info("Enter the method");
        try
        {
            var userRoles = FindRolesByUserId(userId);
            var role = roleService.FindByRoleName("Rater");
            if (!userRoles.Contains(role))
                SaveRoleForUser(userId, role);
        }
        catch (Exception e)
        {
            semLogger.Error("throw exception: " + e.Message);
            Console.Error.WriteLine(e);
        }
        semLogger.Info("Exit method");
    }

    public void RemoveDabirComiteRole(long userId)
    {
        semLogger.Info("Enter the method");
        try
        {
            var userRoles = FindRolesByUserId(userId);
            var role = roleService.FindByRoleName("DabirComite");
            if (role != null && userRoles.Contains(role))
                Delete(FindByUserIdAndRoleId(userId, role.Id));
        }
        catch (Exception e)
        {
            semLogger.Error("throw exception: " + e.Message);
            Console.Error.WriteLine(e);
        }
        semLogger.Info("Exit method");
    }

    public void RemoveRaeesComite(long userId)
    {
        semLogger.Info("Enter the method");
        try
        {
            var userRoles = FindRolesByUserId(userId);
            var role = roleService.FindByRoleName("RaeesComite");
            if (role != null && userRoles.Contains(role))
                Delete(FindByUserIdAndRoleId(userId, role.Id));
        }
        catch (Exception e)
        {
            semLogger.Error("throw exception: " + e.Message);
            Console.Error.WriteLine(e);
        }
        semLogger.Info("Exit method");
    }

    public void Create(UserRoleDto userRoleDto)
    {
        semLogger.Info("Enter the method");
        var user = userService.FindByIdOrElseThrow(userRoleDto.UserId!.Value);
        userRoleDto.RoleDtoList!.ForEach(roleDto =>
        {
            var role = roleService.FindByRoleName(roleDto.RoleName!);
            var found = GetRepository().FindByUserIdAndRoleId(user.UserId, role!.Id);
            if (found == null)
                GetRepository().Save(new UserRole { UserId = user.UserId, RoleId = role.Id });
        });
        semLogger.Info("Exit method");
    }

    public void Edit(UserRoleDto userRoleDto)
    {
        semLogger.Info("Enter the method");
        var user = userService.FindByIdOrElseThrow(userRoleDto.UserId!.Value);
        var role = roleService.FindByRoleName(userRoleDto.RoleName!);
        var userRole = FindByUserIdAndRoleId(user.UserId, role!.Id);
        if (userRole == null)
            Create(userRoleDto);
        semLogger.Info("Exit method");
    }

    public bool DeleteByUser(long userId)
    {
        semLogger.Info("Enter the method");
        try
        {
            GetRepository().DeleteAll(userRoleRepository.FindUserRoles(userId));
            semLogger.Info("Exit method");
            return true;
        }
        catch (Exception e)
        {
            semLogger.Error("throw exception: " + e.Message);
            throw new BusinessException(" خطا در حذف نقش کاربر");
        }
    }

    private static long? ReadNullableLong(JsonObject json, string propertyName)
    {
        var node = json[propertyName];
        if (node == null) return null;
        return node.GetValue<long>();
    }

    public void ChangeTashilgar(string body)
    {
        semLogger.Info("Enter the method");
        var json = JsonNode.Parse(body)!.AsObject();
        long? planId = ReadNullableLong(json, "planId");
        long? oldId = ReadNullableLong(json, "oldTashilgarId");
        long? newId = ReadNullableLong(json, "newTashilgarId");

        measureService.changeTashilgar(oldId, newId, planId);
        if (tenantName.Equals("HOSCO") || tenantName.Equals("IRISA"))
            measureCatalogService.changeTashilgar(oldId, newId, planId);
        actionPlanService.changeTashilgar(oldId, newId, planId);
        if (tenantName.Equals("HOSCO"))
            actionPlanCatalogService.changeTashilgar(oldId, newId, planId);

        AddTashilgarUserRole(newId!.Value);
        var planingLevelIds = planingLevelUserService.GetUserLevelIdsAsync(oldId!.Value).GetAwaiter().GetResult();
        planingLevelUserService.CreateOrDeleteAsync(newId.Value, planingLevelIds).GetAwaiter().GetResult();
        semLogger.Info("Exit method");
    }

    public void ChangeModir(string body)
    {
        semLogger.Info("Enter the method");
        var json = JsonNode.Parse(body)!.AsObject();
        long? planId = ReadNullableLong(json, "planId");
        long? oldId = ReadNullableLong(json, "oldModirId");
        long? newId = ReadNullableLong(json, "newModirId");

        measureService.changeModir(oldId, newId, planId);
        if (tenantName.Equals("HOSCO") || tenantName.Equals("IRISA"))
            measureCatalogService.changeModir(oldId, newId, planId);
        actionPlanService.changeModir(oldId, newId, planId);
        if (tenantName.Equals("HOSCO"))
            actionPlanCatalogService.changeModir(oldId, newId, planId);

        AddModirUserRole(newId!.Value);
        var planingLevelIds = planingLevelUserService.GetUserLevelIdsAsync(oldId!.Value).GetAwaiter().GetResult();
        planingLevelUserService.CreateOrDeleteAsync(newId.Value, planingLevelIds).GetAwaiter().GetResult();
        semLogger.Info("Exit method");
    }

    public List<UserRole> FindUsersByAdminRole() =>
        ReturnOrThrow(userRoleRepository.FindAdminUserRole(), "ادمینی وجود ندارد.");

    public List<UserRole> FindUsersByTashilgarRole() =>
        ReturnOrThrow(userRoleRepository.FindTashilgarUserRoles(), "نقش تسهیلگر تاکنون به کاربری اعطاء نشده است.");

    public List<UserRole> FindUsersBySanayeUserRole() =>
        ReturnOrThrow(userRoleRepository.FindSanayeUserRoles(), "نقش مهندس صنایع تاکنون به کاربری اعطاء نشده است.");

    public List<UserRole> FindUsersBySanayeUserRoleByPlanningLevelId(long planningLevelId) =>
        ReturnOrThrow(userRoleRepository.FindSanayeUserRolesByPlanningLevelId(planningLevelId), "نقش مهندس صنایع تاکنون به کاربری اعطاء نشده است.");

    public List<UserRole> FindUsersByDabirCommittee() =>
        ReturnOrThrow(userRoleRepository.FindDabirComiteUserRoles(), "نقش دبیر کمیته تاکنون به کاربری اعطاء نشده است.");

    public List<UserRole> FindUsersByRaessCommittee() =>
        ReturnOrThrow(userRoleRepository.FindRaeesComiteUserRoles(), "نقش رئیش کمیته تاکنون به کاربری اعطاء نشده است.");

    public List<UserRole> FindUsersByDabirCommitteeAndPlanningLevel(long planningLevelId)
    {
        var result = userRoleRepository.FindDabirCommitteeUserRolesAndPlanningLevel(planningLevelId);
        return result != null ? result : throw new BusinessException("نقش دبیر کمیته تاکنون به کاربری اعطاء نشده است.");
    }

    public List<UserRole> FindUsersByRaessCommitteeAndPlanningLevel(long planningLevelId)
    {
        var result = userRoleRepository.FindRaeesCommitteeUserRolesAndPlanningLevel(planningLevelId);
        return result != null ? result : throw new BusinessException("نقش رئیش کمیته تاکنون به کاربری اعطاء نشده است.");
    }

    public List<UserRole> FindUsersByTaaliUserRole(long planningLevelId) =>
        ReturnOrThrow(userRoleRepository.FindTaaliUserRolesByPlanningLevel(planningLevelId),
            "نقش نماینده تعالی سازمانی تاکنون به کاربری اعطاء نشده است.");

    private static List<UserRole> ReturnOrThrow(List<UserRole> roles, string message) =>
        roles.Count != 0 ? roles : throw new BusinessException(message);
}
