#nullable enable
using Microsoft.EntityFrameworkCore;
using SecurityRole = global::SemBackDotNet10.SecurityDomain.Model.Entity.Role;
using SecurityUserRole = global::SemBackDotNet10.SecurityDomain.Model.Entity.UserRole;

namespace SemBackDotNet10.service.impl;

internal sealed partial class ExternalDomainServices
{
    public Task AddDabirComiteRoleAsync(long userId, CancellationToken ct) => AddRoleAsync(userId, "DabirComite", ct);
    public Task AddModirUserRoleAsync(long userId, CancellationToken ct) => AddRoleAsync(userId, "Modiriat", ct);
    public Task AddTashilgarUserRoleAsync(long userId, CancellationToken ct) => AddRoleAsync(userId, "Tashilgar", ct);
    public Task AddRaeesComiteAsync(long userId, string tenant, CancellationToken ct) =>
        AddRoleAsync(userId, tenant == "IRISA" ? "ModirCommitte" : "RaeesComite", ct);
    public Task RemoveRaeesComiteAsync(long userId, CancellationToken ct) => RemoveRoleAsync(userId, "RaeesComite", ct);
    public Task RemoveDabirComiteRoleAsync(long userId, CancellationToken ct) => RemoveRoleAsync(userId, "DabirComite", ct);

    private async Task AddRoleAsync(long userId, string name, CancellationToken ct)
    {
        var role = await db.Set<SecurityRole>().SingleOrDefaultAsync(x => x.RoleName == name, ct)
            ?? throw new InvalidOperationException($"Role '{name}' was not found.");
        var roles = db.Set<SecurityUserRole>();
        if (roles.Local.Any(x => x.UserId == userId && x.RoleId == role.Id && db.Entry(x).State != EntityState.Deleted)
            || await roles.AnyAsync(x => x.UserId == userId && x.RoleId == role.Id, ct)) return;
        var user = await GetUserAsync(userId, ct);
        roles.Add(new SecurityUserRole { UserId = userId, User = user, RoleId = role.Id, Role = role, CreatedDate = DateTime.UtcNow });
    }

    private async Task RemoveRoleAsync(long userId, string name, CancellationToken ct)
    {
        var roles = db.Set<SecurityUserRole>();
        var matches = await roles.Where(x => x.UserId == userId && x.Role.RoleName == name).ToListAsync(ct);
        roles.RemoveRange(matches);
    }
}
