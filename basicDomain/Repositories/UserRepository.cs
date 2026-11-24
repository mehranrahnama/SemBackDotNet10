#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.baseDomain.repository;

public interface IUserRepository : ISemRepository<User, long>
{
    Task<PageResult<UserDto>> FindAllAsync(PageRequest page, string? filter, CancellationToken ct = default);
    Task<PageResult<User>> FindAll_v2Async(PageRequest page, long levelId, CancellationToken ct = default);
    Task<PageResult<User>> FindAllDataInHosco_v2Async(PageRequest page, long levelId, CancellationToken ct = default);
    Task<PageResult<User>> FindAllWithKeyWordAsync(PageRequest page, string? filter, long levelId, CancellationToken ct = default);
    Task<PageResult<User>> FindAllWithKeyWordHoscoAsync(PageRequest page, string? filter, long levelId, CancellationToken ct = default);
    Task<Tenant?> FindUserTenantByUserNameAsync(string userName, CancellationToken ct = default);
    Task<User?> FindByUserNameAsync(string userName, CancellationToken ct = default);
    Task<User?> FindByExtensionNoAsync(string extentioNo, CancellationToken ct = default);
    Task<User?> FindByAccountUsernameAsync(string username, CancellationToken ct = default);
    Task<long?> CountActiveActionsAsync(long planId, CancellationToken ct = default);
    Task<long?> CountActiveUsersByPlanIdLevelIdAsync(long planId, long levelId, CancellationToken ct = default);
    Task<UserDto?> GetUserDetailsAsync(string accountName, CancellationToken ct = default);
    Task<User?> FindByNationalCodeAsync(string nationalCode, CancellationToken ct = default);
    Task<PageResult<UserDto>> FindAllTashilgarAsync(PageRequest page, string? filter, CancellationToken ct = default);
    Task<PageResult<UserDto>> FindAllModirAsync(PageRequest page, string? filter, CancellationToken ct = default);
}

public sealed class UserRepository(DbContext db)
    : SemRepository<User, long>(db), IUserRepository
{
    public Task<PageResult<UserDto>> FindAllAsync(PageRequest page, string? filter, CancellationToken ct = default)
    {
        var pat = "%" + filter + "%";
        var query = from user in Set
                    join position in Db.Set<Position>() on (long?)user.PositionId equals (long?)position.Id into positions
                    from position in positions.DefaultIfEmpty()
                    join level in Db.Set<PlaningLevel>() on (long?)user.PlaningLevelId equals (long?)level.Id into levels
                    from level in levels.DefaultIfEmpty()
                    where filter == null || EF.Functions.Like(user.UserId.ToString(), pat) ||
                        EF.Functions.Like(user.NameF!, pat) || EF.Functions.Like(user.FamilyF!, pat) ||
                        EF.Functions.Like((user.NameF ?? "") + " " + (user.FamilyF ?? ""), pat) ||
                        (position != null && EF.Functions.Like(position.DesPost!, pat)) ||
                        (level != null && EF.Functions.Like(level.DesLevel!, pat))
                    orderby user.UserId
                    select new UserDto(user.UserId, user.UserName, user.NameF, user.FamilyF,
                        user.Retired == true, user.CellPhoneNo, user.InternalPhoneNum,
                        position == null ? (long?)null : position.Id, position == null ? null : position.DesPost,
                        level == null ? (long?)null : level.Id, level == null ? null : level.DesLevel,
                        user.NationalCode);
        return PageAsync(query, page, ct);
    }

    public Task<PageResult<User>> FindAll_v2Async(PageRequest page, long levelId, CancellationToken ct = default)
    {
        return PageAsync(Set.Where(x => levelId == 0 || Db.Set<PlaningLevelUser>().Any(lu => lu.UserId == x.UserId && lu.LevelId == levelId)).OrderBy(x => x.UserId), page, ct);
    }

    public async Task<PageResult<User>> FindAllDataInHosco_v2Async(PageRequest page, long levelId, CancellationToken ct = default)
    {
        var query = Set.Where(x => levelId == 0 || Db.Set<PlaningLevelUser>()
            .Any(lu => lu.UserId == x.UserId && lu.LevelId == levelId));
        var result = await PageAsync(query.OrderBy(x => x.UserId), page, ct);
        // Match the Java left fetch join without changing the existing FK column type.
        var ids = result.Content.Where(u => u.PositionId.HasValue)
            .Select(u => (long)u.PositionId!.Value).Distinct().ToArray();
        if (ids.Length != 0)
        {
            var positions = await Db.Set<Position>().Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, ct);
            foreach (var user in result.Content)
                user.Position = user.PositionId is int id ? positions.GetValueOrDefault(id) : null;
        }
        return result;
    }

    public Task<PageResult<User>> FindAllWithKeyWordAsync(PageRequest page, string? filter, long levelId, CancellationToken ct = default)
    {
        // A null keyword matches no rows, as SQL LIKE NULL does.
        var pat = filter == null ? null : "%" + filter + "%";
        var q = Set.Where(x => (levelId == 0 || Db.Set<PlaningLevelUser>().Any(lu => lu.UserId == x.UserId && lu.LevelId == levelId)) && filter != null && (EF.Functions.Like(x.UserId.ToString(), pat!) || EF.Functions.Like((x.NameF ?? "") + " " + (x.FamilyF ?? ""), pat!) || Db.Set<PlaningLevelUser>().Any(lu => lu.UserId == x.UserId && lu.Level != null && EF.Functions.Like(lu.Level.DesLevel, pat!)) || EF.Functions.Like(x.NationalCode, pat!)));
        return PageAsync(q.OrderBy(x => x.UserId), page, ct);
    }

    public Task<PageResult<User>> FindAllWithKeyWordHoscoAsync(PageRequest page, string? filter, long levelId, CancellationToken ct = default)
    {
        // A null keyword matches no rows, as SQL LIKE NULL does.
        var pat = filter == null ? null : "%" + filter + "%";
        var q = Set.Where(x => (levelId == 0 || Db.Set<PlaningLevelUser>().Any(lu => lu.UserId == x.UserId && lu.LevelId == levelId)) && filter != null &&
            (EF.Functions.Like(x.UserId.ToString(), pat!) ||
             EF.Functions.Like((x.NameF ?? "") + " " + (x.FamilyF ?? ""), pat!) ||
             Db.Set<Position>().Any(p => p.Id == (long?)x.PositionId && EF.Functions.Like(p.DesPost!, pat!)) ||
             EF.Functions.Like(x.NationalCode!, pat!)));
        return PageAsync(q.OrderBy(x => x.UserId), page, ct);
    }

    public Task<Tenant?> FindUserTenantByUserNameAsync(string userName, CancellationToken ct = default)
    {
        var query = from user in Set
                    where user.UserName == userName
                    join organization in Db.Set<Organization>() on (long?)user.OrganizationId equals (long?)organization.Id
                    where organization.Tenant != null
                    select organization.Tenant;
        return query.SingleOrDefaultAsync(ct);
    }

    public Task<User?> FindByUserNameAsync(string userName, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.UserName == userName, ct);
    }

    public Task<User?> FindByExtensionNoAsync(string extentioNo, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.InternalPhoneNum == extentioNo, ct);
    }

    public Task<User?> FindByAccountUsernameAsync(string username, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.Accounts.Any(a => a.AccountName.ToUpper() == username.ToUpper()), ct);
    }

    public async Task<long?> CountActiveActionsAsync(long planId, CancellationToken ct = default)
    {
        var values = await Db.Database.SqlQuery<long?>($"EXEC [dbo].[CountActiveUsers] {planId}").ToListAsync(ct);
        return values.SingleOrDefault();
    }

    public async Task<long?> CountActiveUsersByPlanIdLevelIdAsync(long planId, long levelId, CancellationToken ct = default)
    {
        var values = await Db.Database.SqlQuery<long?>($"EXEC [dbo].[CountActiveUsersByPlanIdLevelId] {planId}, {levelId}").ToListAsync(ct);
        return values.SingleOrDefault();
    }

    public Task<UserDto?> GetUserDetailsAsync(string accountName, CancellationToken ct = default)
    {
        var query = from account in Db.Set<SemBackDotNet10.SecurityDomain.Model.Entity.UserAccount>()
                    where account.AccountName == accountName
                    join user in Set on account.UserId equals (long?)user.UserId into users
                    from user in users.DefaultIfEmpty()
                    join position in Db.Set<Position>() on (long?)user.PositionId equals (long?)position.Id into positions
                    from position in positions.DefaultIfEmpty()
                    select new UserDto(user == null ? null : (long?)user.UserId, account.Id,
                        user == null ? null : user.NameF, user == null ? null : user.FamilyF,
                        user == null ? null : user.UserName, position == null ? null : position.DesPost,
                        user == null ? null : user.LastFailedLoginAttempt,
                        user == null ? null : user.LastSuccessfulLoginAttempt);
        return query.SingleOrDefaultAsync(ct);
    }

    public Task<User?> FindByNationalCodeAsync(string nationalCode, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.NationalCode == nationalCode, ct);
    }

    public Task<PageResult<UserDto>> FindAllTashilgarAsync(PageRequest page, string? filter, CancellationToken ct = default)
        => FindByRoleAsync(page, filter, "Tashilgar", ct);

    public Task<PageResult<UserDto>> FindAllModirAsync(PageRequest page, string? filter, CancellationToken ct = default)
        => FindByRoleAsync(page, filter, "Modiriat", ct);

    private Task<PageResult<UserDto>> FindByRoleAsync(PageRequest page, string? filter, string roleName, CancellationToken ct)
    {
        var users = from ur in Db.Set<UserRoleEntity>()
                    join u in Set on ur.UserId equals u.UserId
                    where ur.Role!.RoleName == roleName &&
                          (filter == null || EF.Functions.Like((u.NameF ?? "") + " " + (u.FamilyF ?? ""), "%" + filter + "%"))
                    orderby u.UserId
                    select new UserDto(u.UserId, u.NameF, u.FamilyF);
        return PageAsync(users, page, ct);
    }
}
