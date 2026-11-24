using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Persistence;

public sealed class RoleRepository(AppDbContext db) : IRoleRepository
{
    public async Task<IReadOnlyList<(long Id, string Name)>> GetForUserAsync(long tenantId, long userId, CancellationToken cancellationToken = default)
    {
        var roles = await db.UserRoles
            .AsNoTracking()
            .Where(userRole => userRole.UserId == userId && userRole.User.TenantId == tenantId)
            .Select(userRole => new { userRole.RoleId, userRole.Role.RoleName })
            .ToListAsync(cancellationToken);

        return roles.Select(role => (role.RoleId, role.RoleName ?? string.Empty)).ToArray();
    }
}