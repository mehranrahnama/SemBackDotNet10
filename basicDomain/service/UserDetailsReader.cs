using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Infrastructure.Security;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Persistence;
using UserDetailsDto = SemBackDotNet10.basicDomain.Model.Dto.UserDto;

namespace SemBackDotNet10.service.impl;

public sealed class UserDetailsReader(AppDbContext db, UserContext current, IRoleRepository roles)
{
    public async Task<UserDetailsDto?> GetAsync(string accountName, CancellationToken ct)
    {
        const string suffix = "_IRISA_LDAP";
        var account = await db.UserAccounts.AsNoTracking().Include(item => item.User).Include(item => item.Provider)
            .FirstOrDefaultAsync(item => item.AccountName == accountName && item.Provider != null && item.Provider.TenantId == current.TenantId, ct);
        if (account is null && accountName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            var alternate = accountName[..^suffix.Length];
            account = await db.UserAccounts.AsNoTracking().Include(item => item.User).Include(item => item.Provider)
                .FirstOrDefaultAsync(item => item.AccountName == alternate && item.Provider != null && item.Provider.TenantId == current.TenantId, ct);
        }
        if (account?.User is null || account.User.UserId != current.UserId || account.User.TenantId != current.TenantId)
            return null;
        var user = account.User;
        string? position = null;
        if (user.PositionId is int positionId)
            position = await db.Database.SqlQuery<string>($"SELECT [DesPost] AS [Value] FROM [dbo].[tblPosition] WHERE [PositionId] = {positionId}").FirstOrDefaultAsync(ct);
        var userRoles = await roles.GetForUserAsync(current.TenantId, user.UserId, ct);
        return new UserDetailsDto(user.UserId, account.Id, user.NameF, user.FamilyF, user.UserName,
            position, user.LastFailedLoginAttempt, user.LastSuccessfulLoginAttempt)
        {
            Roles = userRoles.Select(role => (RoleDto?)new RoleDto { Id = role.Id, RoleName = role.Name }).ToList()
        };
    }
}
