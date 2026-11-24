using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;

namespace SemBackDotNet10.SecurityDomain.Service.Interf;

/// <summary>معادل OauthProviderService.java — اینترفیس سرویس OAuth/LDAP provider ها.</summary>
public interface IOauthProviderService
{
    /// <summary>معادل متد BaseService: getRepository().</summary>
    IOauthProviderRepository GetRepository();

    /// <summary>معادل findByIdOrElseThrow(Long).</summary>
    Task<OauthProvider> FindByIdOrElseThrowAsync(long id, CancellationToken ct = default);

    /// <summary>معادل deleteOauthProvider(Long).</summary>
    Task DeleteOauthProviderAsync(long id, CancellationToken ct = default);

    /// <summary>معادل findAllProviderInfo() با readOnly.</summary>
    Task<List<OauthProviderDto>> FindAllProviderInfoAsync(CancellationToken ct = default);

    /// <summary>معادل findByRegistrationId(String).</summary>
    Task<OauthProvider> FindByRegistrationIdAsync(string registrationId, CancellationToken ct = default);

    /// <summary>معادل findByTenantIdAndType(Long, String).</summary>
    Task<OauthProvider?> FindByTenantIdAndTypeAsync(long tenantId, string type, CancellationToken ct = default);

    /// <summary>معادل findByType(String).</summary>
    Task<OauthProvider> FindByTypeAsync(string type, CancellationToken ct = default);

    /// <summary>معادل findLdapProvider().</summary>
    Task<OauthProvider?> FindLdapProviderAsync(CancellationToken ct = default);
}
