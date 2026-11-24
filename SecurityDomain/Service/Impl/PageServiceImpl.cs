using Common.logging;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;
using SemBackDotNet10.SecurityDomain.Service.Interf;


namespace SemBackDotNet10.SecurityDomain.Service.Impl;

/// <summary>
/// معادل PageServiceImpl.java.
/// وابستگی‌های اعلام‌شده: IPageRepository، IUserRoleService (CIRCULAR — با Lazy حل شود)،
///   SecurityUtils.GetCurrentUser، PageDto/PageAccessDto، SemLogger — هنوز تبدیل نشده‌اند.
/// </summary>
public class PageServiceImpl : IPageService
{
    private readonly IPageRepository _pageRepository;
    private readonly IUserRoleService _userRoleService; //CIRCULAR
    private readonly SemLogger _semLogger;

    public PageServiceImpl(IPageRepository pageRepository, IUserRoleService userRoleService, SemLogger semLogger)
    {
        _pageRepository = pageRepository;
        _userRoleService = userRoleService;
        _semLogger = semLogger;
    }

    public IPageRepository GetRepository() => _pageRepository;

    public List<PageDto> GetListByRoleId(long roleId)
    {
        _semLogger.Info("Enter the method");
        var pageDtoList = GetRepository().GetMenuTree(roleId);
        SetPageChildren(pageDtoList);
        _semLogger.Info("Exit method");
        return pageDtoList;
    }

    private void SetPageChildren(List<PageDto> pageList)
    {
        _semLogger.Info("Enter the method");
        // create tree
        foreach (var page in pageList)
        {
            var children = new List<PageDto>();
            foreach (var child in pageList)
            {
                if (child.ParentId == page.Id)
                    children.Add(child);
            }
            page.Children = children;
        }

        // remove children from root
        for (var i = pageList.Count - 1; i >= 0; i--)
        {
            if (pageList[i].ParentId != 0)
                pageList.RemoveAt(i);
        }
        _semLogger.Info("Exit method");
    }

    private static long RequireCurrentUserId() => SecurityUtils.GetCurrentUserId()
        ?? throw new global::SemBackDotNet10.Common.exception.UnAuthorizedException();

    public List<string> GetPageNameByUserId()
    {
        _semLogger.Info("Enter the method");
        var userId = RequireCurrentUserId();
        var result = _pageRepository.GetPageNameByUserId(userId);
        _semLogger.Info("Exit method");
        return result;
    }

    public List<string> GetUriByUserId()
    {
        var userId = RequireCurrentUserId();
        return _pageRepository.GetUriByUserId(userId);
    }

    private bool UserIsAdmin(long userId)
    {
        _semLogger.Info("Enter the method");
        var result = _userRoleService.FindUserRoles(userId)
            .Select(userRole => userRole.Role!.RoleName)
            .Contains("Administrators");
        _semLogger.Info("Exit method");
        return result;
    }

    public List<PageAccessDto> GetpageAllDataByUserId()
    {
        _semLogger.Info("Enter the method");
        var userId = RequireCurrentUserId();
        if (UserIsAdmin(userId))
        {
            var result = _pageRepository.GetpageAllData();
            _semLogger.Info("Exit method");
            return result;
        }
        var result2 = _pageRepository.GetpageAllDataByUserId(userId);
        _semLogger.Info("Exit method");
        return result2;
    }

    public PageDto MapToDto(Page page)
    {
        _semLogger.Info("Enter the method");
        var pageDto = new PageDto
        {
            DesF = page.DesF,
            Id = page.Id,
            ParentId = page.ParentId
        };
        _semLogger.Info("Exit method");
        return pageDto;
    }

    public List<PageDto> MapToDtoList(List<Page> pageList)
    {
        var pageDtoList = new List<PageDto>();
        pageList.ForEach(page => { pageDtoList.Add(MapToDto(page)); });
        return pageDtoList;
    }

    public Page FindByIdOrElseThrow(long? id)
    {
        _semLogger.Info("Enter the method");
        if (id == null)
        {
            throw new BusinessException("error.page.idIsNotValid");
        }
        var result = GetRepository().FindById(id.Value)
            ?? throw new BusinessException("error.page.idIsNotValid");
        _semLogger.Info("Exit method");
        return result;
    }
}
