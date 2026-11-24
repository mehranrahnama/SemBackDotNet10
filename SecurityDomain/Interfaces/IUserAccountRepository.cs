using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface IUserAccountRepository
{
    Task<UserAccount?> FindJdbcAccountAsync(long tenantId, string username, CancellationToken cancellationToken = default);

    Task<UserAccount?> FindByUsernameAsync(long tenantId, string username, CancellationToken cancellationToken = default);

    Task<UserAccount?> FindByProviderAndUsernameAsync(long tenantId, long providerId, string username, CancellationToken cancellationToken = default);

    Task<UserAccount?> FindByUserIdAsync(long tenantId, long userId, CancellationToken cancellationToken = default);

    Task AddAsync(UserAccount account, CancellationToken cancellationToken = default);
}
