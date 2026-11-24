using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Providers
{
    public interface IAuthProviderRepository
    {
        Task<IReadOnlyList<AuthProviderEntity>>
            GetProvidersByTenantAsync(
                long tenantId,
                CancellationToken cancellationToken = default);
    }
}
