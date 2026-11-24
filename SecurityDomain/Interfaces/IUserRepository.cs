using SemBackDotNet10.SecurityDomain.Model.Entity;
using SecurityUser = SemBackDotNet10.SecurityDomain.Model.Entity.User;

namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface IUserRepository
{
    Task<SecurityUser?> FindByIdAsync(long tenantId, long userId, CancellationToken cancellationToken = default);

    Task<SecurityUser?> FindByUsernameAsync(long tenantId, string username, CancellationToken cancellationToken = default);

    Task<SecurityUser?> FindByExternalSubjectAsync(long tenantId, string subject, CancellationToken cancellationToken = default);
}