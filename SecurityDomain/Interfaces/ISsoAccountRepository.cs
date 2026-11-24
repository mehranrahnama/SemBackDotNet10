using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface ISsoAccountRepository
{
    Task<SsoSynchronizationResult> SynchronizeAsync(
        long tenantId,
        SsoUserDto user,
        CancellationToken cancellationToken = default);
}
