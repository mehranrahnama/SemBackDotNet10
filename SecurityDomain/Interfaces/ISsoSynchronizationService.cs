using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface ISsoSynchronizationService
{
    Task<SsoSynchronizationResult> SynchronizeAsync(string username, string accessToken, CancellationToken cancellationToken = default);
}
