using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

/// <summary>معادل PageAccessService.java — اینترفیس سرویس دسترسی صفحه‌ها.</summary>
public interface IPageAccessService
{
    /// <summary>معادل متد BaseService: getRepository().</summary>
    IPageAccessRepository GetRepository();

    /// <summary>معادل findPageIdsByRoleId(Long).</summary>
    List<long> FindPageIdsByRoleId(long roleId);

    /// <summary>معادل createOrDelete(Long, List&lt;Long&gt;).</summary>
    void CreateOrDelete(long roleId, List<long> pageIdList);

    /// <summary>معادل createDefaultPageAccess(Long).</summary>
    void CreateDefaultPageAccess(long roleId);
}
