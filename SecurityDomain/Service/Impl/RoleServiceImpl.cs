
using Common.exception;
using Common.logging;
using Common.utils;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;
using SemBackDotNet10.SecurityDomain.Service.Interf;
using System.Text.Json.Nodes;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

/// <summary>
/// معادل RoleServiceImpl.java.
/// IPageAccessService به‌صورت Lazy دریافت می‌شود تا وابستگی دوطرفه با PageAccessServiceImpl
/// مانند @Lazy در نسخه جاوا هنگام ساخت سرویس‌ها مشکل ایجاد نکند.
/// </summary>
public class RoleServiceImpl : IRoleService
{
    private readonly IPageAccessRepository _pageAccessRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly Lazy<IPageAccessService> _pageAccessService;
    private readonly SemLogger _semLogger;

    public RoleServiceImpl(IRoleRepository roleRepository, IServiceProvider serviceProvider,
        IPageAccessRepository pageAccessRepository, SemLogger semLogger)
    {
        _pageAccessRepository = pageAccessRepository;
        _roleRepository = roleRepository;
        _pageAccessService = new Lazy<IPageAccessService>(
            serviceProvider.GetRequiredService<IPageAccessService>);
        _semLogger = semLogger;
    }

    public IRoleRepository GetRepository() => _roleRepository;

    public SearchDto FindAll(string filter, int page, int size, string? orderBy, string? direction)
    {
        _semLogger.Info("Enter the method");
        var json = JsonNode.Parse(filter) as JsonObject;
        var keyWord = json?["filter"]?.GetValue<string>() ?? "";
        SortDirection? sortDirection = Enum.TryParse<SortDirection>(direction, true, out var parsedDirection)
            ? parsedDirection
            : null;
        var pageable = PageUtils.GetInstance(page, size, orderBy, sortDirection);
        var rolesPage = _roleRepository.FindAllRoles(pageable, keyWord);

        _semLogger.Info("Exit method");
        return new SearchDto.Builder()
            .Content(rolesPage.Content.Cast<object?>().ToList())
            .TotalElements(rolesPage.TotalElements)
            .TotalPages(rolesPage.TotalPages)
            .CurrentPage(page)
            .PageSize(size)
            .Build();
    }

    public List<RoleDto> GetList()
    {
        _semLogger.Info("Enter the method");
        var result = _roleRepository.GetList().Select(MapToDto).ToList();
        _semLogger.Info("Exit method");
        return result;
    }

    public RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            RoleName = role.RoleName,
            RoleDes = role.RoleDes
        };
    }

    public Role MapToEntity(RoleDto roleDto)
    {
        _semLogger.Info("Enter the method");
        var role = new Role
        {
            Id = roleDto.Id ?? 0,
            RoleName = roleDto.RoleName,
            RoleDes = roleDto.RoleDes
        };
        _semLogger.Info("Exit method");
        return role;
    }

    public Role Create(RoleDto roleDto)
    {
        _semLogger.Info("Enter the method");
        ArgumentNullException.ThrowIfNull(roleDto);
        var role = Save(MapToEntity(roleDto));
        _pageAccessService.Value.CreateDefaultPageAccess(role.Id);
        _semLogger.Info("Exit method");
        return role;
    }

    public Role Edit(RoleDto roleDto)
    {
        _semLogger.Info("Enter the method");
        ArgumentNullException.ThrowIfNull(roleDto);
        var role = FindByIdOrElseThrow(roleDto.Id);
        role.RoleName = roleDto.RoleName;
        role.RoleDes = roleDto.RoleDes;
        var result = Save(role);
        _semLogger.Info("Exit method");
        return result;
    }

    public List<RoleDto> GetRolesByUserId(long userId)
    {
        return GetRepository().GetRolesByUserId(userId).Select(MapToDto).ToList();
    }

    public void Delete(long id, List<long> defaultPageIds)
    {
        _semLogger.Info("Enter the method");
        var role = FindByIdOrElseThrow(id);
        var pageAccesses = _pageAccessRepository.FindPageAccessByRoleId(id);
        if (pageAccesses.Count <= defaultPageIds.Count)
        {
            _pageAccessRepository.DeleteAll(pageAccesses);
        }
        else
        {
            throw new BusinessException("این نقش به منو ها منتصب شده است.");
        }

        try
        {
            Delete(role);
        }
        catch (Exception e)
        {
            _semLogger.Error("throw exception: " + e.Message);
            throw new BusinessException("این نقش به کاربران منتصب شده است.");
        }
        _semLogger.Info("Exit method");
    }

    public Role FindByIdOrElseThrow(long? id)
    {
        _semLogger.Info("Enter the method");
        if (id == null)
        {
            throw new BusinessException("error.role.idIsNotValid");
        }
        var result = GetRepository().FindById(id.Value)
            ?? throw new BusinessException("error.role.idIsNotValid");
        _semLogger.Info("Exit method");
        return result;
    }

    public Role? FindByRoleName(string roleName)
    {
        _semLogger.Info("Enter the method");
        var result = _roleRepository.FindByRoleName(roleName);
        _semLogger.Info("Exit method");
        return result;
    }

    private Role Save(Role role) => _roleRepository.Save(role);

    private void Delete(Role role) => _roleRepository.Delete(role);
}
