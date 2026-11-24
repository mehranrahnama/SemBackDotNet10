using global::SemBackDotNet10.SecurityDomain.Service.Interf;
using SemLogger = global::Common.logging.SemLogger;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

/// <summary>
/// معادل PageAccessServiceImpl.java.
/// وابستگی‌های اعلام‌شده: IPageAccessRepository، IPageRepository، IRoleService (FindByIdOrElseThrow)،
///   IPageService (FindByIdOrElseThrow)، SemLogger — هنوز تبدیل نشده‌اند.
/// </summary>
public class PageAccessServiceImpl : IPageAccessService
{
    private readonly IPageAccessRepository _pageAccessRepository;
    private readonly IRoleService _roleService; //CIRCULAR — در دات‌نت با Lazy<T> یا IDResolver حل شود
    private readonly IPageService _pageService;
    private readonly IPageRepository _pageRepository;
    private readonly SemLogger _semLogger;

    public PageAccessServiceImpl(IPageAccessRepository pageAccessRepository, IRoleService roleService,
        IPageService pageService, IPageRepository pageRepository, SemLogger semLogger)
    {
        _pageAccessRepository = pageAccessRepository;
        _roleService = roleService;
        _pageService = pageService;
        _pageRepository = pageRepository;
        _semLogger = semLogger;
    }

    public IPageAccessRepository GetRepository() => _pageAccessRepository;

    public List<long> FindPageIdsByRoleId(long roleId)
    {
        return _pageAccessRepository.FindPageIdsByRoleId(roleId);
    }

    public void CreateOrDelete(long roleId, List<long> pageIds)
    {
        _semLogger.Info("Enter the method");
        var foundPageIds = GetRepository().FindPageIdsByRoleId(roleId);
        var role = _roleService.FindByIdOrElseThrow(roleId);

        // save
        foreach (var pageId in pageIds)
        {
            if (!foundPageIds.Contains(pageId))
            {
                var page = _pageService.FindByIdOrElseThrow(pageId);
                var pageAccess = new PageAccess
                {
                    Page = page,
                    Role = role
                };
                Save(pageAccess);
            }
        }

        // delete
        foreach (var foundPageId in foundPageIds)
        {
            if (!pageIds.Contains(foundPageId))
                Delete(FindByRoleIdAndPageId(roleId, foundPageId));
        }
        _semLogger.Info("Exit method");
    }

    public void CreateDefaultPageAccess(long roleId)
    {
        _semLogger.Info("Enter the method");
        var defaultPage = new List<string> { "Home", "Cartable", "StrategyMap" };
        var pageIds = _pageRepository.FindByPageName(defaultPage);
        CreateOrDelete(roleId, pageIds);
        _semLogger.Info("Exit method");
    }

    private PageAccess FindByRoleIdAndPageId(long roleId, long pageId)
    {
        _semLogger.Info("Enter the method");
        var result = GetRepository().FindByRoleIdAndPageId(roleId, pageId);
        _semLogger.Info("Exit method");
        return result;
    }

    //----------معادل CrudService پایه‌ی جاوا----------

    private void Save(PageAccess entity) => GetRepository().Save(entity);

    private void Delete(PageAccess entity) => GetRepository().Delete(entity);
}
