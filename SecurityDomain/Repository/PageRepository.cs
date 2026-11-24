using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Repository;

public interface IPageRepository
{
    List<PageDto> GetMenuTree(long roleId);

    List<string> GetPageNameByUserId(long userId);

    List<string> GetUriByUserId(long userId);

    List<PageAccessDto> GetpageAllDataByUserId(long userId);

    List<PageAccessDto> GetpageAllData();

    Page? FindByUri(string uri);

    List<long> FindByPageName(List<string> pageName);

    Page Save(Page entity);

    void Delete(Page entity);

    Page? FindById(long id);

    List<Page> FindAll();
}

public class PageRepository : SecurityRepositoryBase<Page>, IPageRepository
{
    private readonly AppDbContext _db;

    public PageRepository(AppDbContext context) : base(context)
    {
        _db = context;
    }

    public List<PageDto> GetMenuTree(long roleId) =>
        Set.Select(p => new PageDto
        {
            Id = p.Id,
            ParentId = p.ParentId,
            DesF = p.DesF,
            IsChecked = _db.Set<PageAccess>().Any(pa => pa.Page.Id == p.Id && pa.Role.Id == roleId)
        }).ToList();

    public List<string> GetPageNameByUserId(long userId) =>
        PagesOfUser(userId).Select(p => p.PageName).ToList();

    public List<string> GetUriByUserId(long userId) =>
        PagesOfUser(userId).Select(p => p.Uri).ToList();

    public List<PageAccessDto> GetpageAllDataByUserId(long userId) =>
        PagesOfUser(userId)
            .Select(p => new PageAccessDto { Id = p.Id, DesF = p.DesF, Uri = p.Uri, PageName = p.PageName })
            .ToList();

    public List<PageAccessDto> GetpageAllData() =>
        Set.Select(p => new PageAccessDto { Id = p.Id, DesF = p.DesF, Uri = p.Uri, PageName = p.PageName }).ToList();

    public Page? FindByUri(string uri) => Set.FirstOrDefault(p => p.Uri == uri);

    public List<long> FindByPageName(List<string> pageName) =>
        Set.Where(p => pageName.Contains(p.PageName)).Select(p => p.Id).ToList();

    private IQueryable<Page> PagesOfUser(long userId) =>
        Set.Where(p => _db.Set<PageAccess>().Any(pa =>
            pa.Page.Id == p.Id &&
            _db.Set<UserRole>().Any(ur => ur.User.UserId == userId && ur.Role.Id == pa.Role.Id)));
}
