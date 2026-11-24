using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Repository;

public interface IPageAccessRepository
{
    List<long> FindPageIdsByRoleId(long roleId);

    List<PageAccess> FindPageAccessByRoleId(long roleId);

    PageAccess? FindByRoleIdAndPageId(long roleId, long pageId);

    List<PageAccess> FindAllByPageIdAndRoleContains(long pageId, List<long> roleIds);

    PageAccess Save(PageAccess entity);

    List<PageAccess> SaveAll(IEnumerable<PageAccess> entities);

    void Delete(PageAccess entity);

    void DeleteAll(IEnumerable<PageAccess> entities);

    PageAccess? FindById(long id);
}

public class PageAccessRepository : SecurityRepositoryBase<PageAccess>, IPageAccessRepository
{
    public PageAccessRepository(AppDbContext context) : base(context) { }

    public List<long> FindPageIdsByRoleId(long roleId) =>
        Set.Where(pa => pa.Role.Id == roleId).Select(pa => pa.Page.Id).ToList();

    public List<PageAccess> FindPageAccessByRoleId(long roleId) =>
        Set.Include(pa => pa.Page).Include(pa => pa.Role).Where(pa => pa.Role.Id == roleId).ToList();

    public PageAccess? FindByRoleIdAndPageId(long roleId, long pageId) =>
        Set.FirstOrDefault(pa => pa.Page.Id == pageId && pa.Role.Id == roleId);

    public List<PageAccess> FindAllByPageIdAndRoleContains(long pageId, List<long> roleIds) =>
        Set.Where(pa => pa.Page.Id == pageId && roleIds.Contains(pa.Role.Id)).ToList();
}
