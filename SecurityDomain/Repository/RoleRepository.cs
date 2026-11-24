using Microsoft.EntityFrameworkCore;
using Common.utils;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Repository;

public interface IRoleRepository
{
    PageResult<RoleDto> FindAllRoles(PageRequest pageable, string? filter);

    List<Role> GetList();

    List<Role> GetRolesByUserId(long userId);

    Role? FindByRoleName(string roleName);

    Role Save(Role entity);

    void Delete(Role entity);

    Role? FindById(long id);
}

public class RoleRepository : SecurityRepositoryBase<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public PageResult<RoleDto> FindAllRoles(PageRequest pageable, string? filter)
    {
        var query = Set.AsQueryable();
        if (!string.IsNullOrEmpty(filter))
            query = query.Where(r => r.RoleName.Contains(filter));

        var total = query.LongCount();
        var ordered = pageable.OrderBy is null ? query.OrderBy(r => r.Id) : query;
        var content = PageUtils.Apply(ordered, pageable)
            .Select(r => new RoleDto { Id = r.Id, RoleName = r.RoleName, RoleDes = r.RoleDes })
            .ToList();
        return ToPage(content, total, pageable.Size);
    }

    public List<Role> GetList() => Set.ToList();

    public List<Role> GetRolesByUserId(long userId) =>
        Context.Set<UserRole>().Where(ur => ur.User.UserId == userId).Select(ur => ur.Role).ToList();

    public Role? FindByRoleName(string roleName) => Set.FirstOrDefault(r => r.RoleName == roleName);
}
