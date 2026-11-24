using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface IOAuthProviderRepository
{
    Task<IReadOnlyList<AuthProviderEntity>> GetOidcProvidersAsync(
        long tenantId,
        CancellationToken cancellationToken = default);
}
