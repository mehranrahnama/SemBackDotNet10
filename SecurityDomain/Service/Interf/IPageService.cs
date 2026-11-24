using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

/// <summary>معادل PageService.java — اینترفیس سرویس صفحه‌ها (منو).</summary>
public interface IPageService
{
    /// <summary>معادل متد BaseService: getRepository().</summary>
    IPageRepository GetRepository();

    /// <summary>معادل getListByRoleId(Long).</summary>
    List<PageDto> GetListByRoleId(long roleId);

    /// <summary>معادل mapToDto(Page).</summary>
    PageDto MapToDto(Page page);

    /// <summary>معادل mapToDtoList(List&lt;Page&gt;).</summary>
    List<PageDto> MapToDtoList(List<Page> pageList);

    /// <summary>معادل findByIdOrElseThrow(Long).</summary>
    Page FindByIdOrElseThrow(long? pageId);

    /// <summary>معادل getPageNameByUserId().</summary>
    List<string> GetPageNameByUserId();

    /// <summary>معادل getUriByUserId().</summary>
    List<string> GetUriByUserId();

    /// <summary>معادل getpageAllDataByUserId().</summary>
    List<PageAccessDto> GetpageAllDataByUserId();
}
