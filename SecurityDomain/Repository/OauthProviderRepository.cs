using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Repository;

public interface IOauthProviderRepository
{
    Task<List<OauthProvider>> FindAllAsync(CancellationToken ct = default);
    Task<OauthProvider?> FindByRegistrationIdAsync(string registrationId, CancellationToken ct = default);

    Task<OauthProvider?> FindByTenantIdAndTypeAsync(long tenantId, string type, CancellationToken ct = default);

    Task<OauthProvider?> FindByTypeAsync(string type, CancellationToken ct = default);

    Task<OauthProvider?> FindByIdAsync(long id, CancellationToken ct = default);

    Task DeleteAsync(OauthProvider entity, CancellationToken ct = default);
}

public class OauthProviderRepository : SecurityRepositoryBase<OauthProvider>, IOauthProviderRepository
{
    public OauthProviderRepository(AppDbContext context) : base(context) { }

    public Task<OauthProvider?> FindByRegistrationIdAsync(string registrationId, CancellationToken ct = default) =>
        Set.FirstOrDefaultAsync(p => p.RegistrationId == registrationId, ct);

    public Task<OauthProvider?> FindByTenantIdAndTypeAsync(long tenantId, string type, CancellationToken ct = default) =>
        Set.FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Type == type, ct);

    public Task<OauthProvider?> FindByTypeAsync(string type, CancellationToken ct = default) =>
        Set.FirstOrDefaultAsync(p => p.Type == type, ct);
}
