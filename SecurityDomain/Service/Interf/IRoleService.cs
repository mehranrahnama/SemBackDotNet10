
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

/// <summary>معادل RoleService.java — اینترفیس سرویس نقش‌ها.</summary>
public interface IRoleService
{
    /// <summary>معادل متد BaseService: getRepository().</summary>
    IRoleRepository GetRepository();

    /// <summary>معادل findAll(filter, page, size, orderBy, direction) — direction جاوا (SortDirection direction) با string شبیه‌سازی شده ("ASC"/"DESC").</summary>
    SearchDto FindAll(string filter, int page, int size, string? orderBy, string? direction);

    /// <summary>معادل mapToDto(Role).</summary>
    RoleDto MapToDto(Role role);

    /// <summary>معادل mapToEntity(RoleDto).</summary>
    Role MapToEntity(RoleDto roleDto);

    /// <summary>معادل findByIdOrElseThrow(Long).</summary>
    Role FindByIdOrElseThrow(long? id);

    /// <summary>معادل create(RoleDto).</summary>
    Role Create(RoleDto roleDto);

    /// <summary>معادل delete(Long, List&lt;Long&gt;).</summary>
    void Delete(long id, List<long> defaultPageIds);

    /// <summary>معادل getList().</summary>
    List<RoleDto> GetList();

    /// <summary>معادل edit(RoleDto).</summary>
    Role Edit(RoleDto roleDto);

    /// <summary>معادل getRolesByUserId(Long).</summary>
    List<RoleDto> GetRolesByUserId(long userId);

    /// <summary>معادل findByRoleName(String).</summary>
    Role? FindByRoleName(string roleName);
}
