using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Persistence;
using SecurityUser = SemBackDotNet10.SecurityDomain.Model.Entity.User;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Persistence;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<SecurityUser?> FindByIdAsync(long tenantId, long userId, CancellationToken cancellationToken = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.TenantId == tenantId && user.UserId == userId, cancellationToken);

    public Task<SecurityUser?> FindByUsernameAsync(long tenantId, string username, CancellationToken cancellationToken = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.TenantId == tenantId && user.UserName == username, cancellationToken);

    public Task<SecurityUser?> FindByExternalSubjectAsync(long tenantId, string subject, CancellationToken cancellationToken = default) =>
        Guid.TryParse(subject, out var externalId)
            ? db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.TenantId == tenantId && user.OldUserId == externalId, cancellationToken)
            : Task.FromResult<SecurityUser?>(null);
}