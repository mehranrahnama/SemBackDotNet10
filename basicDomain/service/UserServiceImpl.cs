#nullable enable
using Common.exception;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.service.impl;

public sealed class UserServiceImpl(ServiceRuntime runtime, IRemainingServiceQueries queries, IRemainingExternalServices external) : ServiceBase<User>(runtime)
{
    public Task<User> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default) => RequireAsync<User>(id, "error.user.idIsNotValid", ct);
    public Task<User?> FindByUserNameAsync(string name, CancellationToken ct = default) => GetRepository().SingleOrDefaultAsync(x => x.UserName == name, ct);
    public Task<Tenant?> FindUserTenantByUserNameAsync(string name, CancellationToken ct = default) => queries.UserTenantAsync(name, ct);
    private void Admin() { if (!R.User.IsAdmin) throw new UnauthorizedAccessException(); }
    private static void Validate(UserDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (!string.IsNullOrEmpty(dto.NationalCode) && dto.NationalCode.Length != 10) throw new BusinessException("تعداد ارقام کدملی صحیح نیست.");
    }
    private async Task CheckLevelsAsync(UserDto dto, CancellationToken ct)
    {
        if (dto.PlaningLevelDtoList != null) foreach (var level in dto.PlaningLevelDtoList) await AccessAsync(level.Id, ct);
        if (dto.PlaningLevelId != null) await AccessAsync(dto.PlaningLevelId, ct);
    }
    private async Task ApplyAsync(User x, UserDto dto, CancellationToken ct)
    {
        x.CellPhoneNo = dto.CellPhoneNo; x.InternalPhoneNum = dto.ExtensionNo; x.NationalCode = dto.NationalCode;
        x.NameF = dto.NameF; x.FamilyF = dto.FamilyF;
        if (dto.PlaningLevelId != null)
        {
            var level = await RequireAsync<PlaningLevel>(dto.PlaningLevelId, "error.planingLevel.idIsNotValid", ct);
            x.PlaningLevelId = checked((int)level.Id);
        }
        var position = dto.PostId == null ? null : await RequireAsync<Position>(dto.PostId, "error.position.idIsNotValid", ct);
        x.PositionId = position == null ? null : checked((int)position.Id);
    }
    public Task<User> EditAsync(UserDto dto, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        Validate(dto); Admin(); await CheckLevelsAsync(dto, ct);
        var user = await FindByIdOrElseThrowAsync(dto.Id, ct); await ApplyAsync(user, dto, ct); return await SaveAsync(user, ct);
    }, ct);
    public UserDto MapToDto(User user) => new()
    {
        Id = user.UserId, UserName = user.UserName, NameF = user.NameF, FamilyF = user.FamilyF,
        Retired = user.Retired == true, CellPhoneNo = user.CellPhoneNo, ExtensionNo = user.InternalPhoneNum,
        PersonnelNumber = (long?)user.PersonnelNumber ?? user.UserId
    };
    public async Task<UserDto> MapToDtoV2Async(User user, CancellationToken ct = default)
    {
        var position = user.PositionId == null ? null : await R.Db.Set<Position>()
            .Where(p => p.Id == (long?)user.PositionId).Select(p => p.DesPost).SingleOrDefaultAsync(ct);
        var dto = MapToDto(user); dto.Fullname = $"{user.NameF} {user.FamilyF}"; dto.NationalCode = user.NationalCode;
        dto.DesPost = position ?? "";
        var memberships = await R.Db.Set<PlaningLevelUser>().AsNoTracking()
            .Where(x => x.UserId == user.UserId && x.Level != null)
            .Include(x => x.Level).ThenInclude(x => x!.Parent)
            .Include(x => x.Level).ThenInclude(x => x!.Responsible)
            .Include(x => x.Level).ThenInclude(x => x!.Facilitator)
            .ToListAsync(ct);
        dto.PlaningLevelDtoList = memberships.Select(x => PlaningLevelServiceImpl.MapLevelToDto(x.Level!)).ToList();
        dto.Roles = await queries.UserRolesAsync(user.UserId, ct); return dto;
    }
    public async Task<ServicePage<UserDto>> FindAllAsync(string? filter, long? levelId, ServicePageRequest page, CancellationToken ct = default)
    {
        if (levelId != null) await AccessAsync(levelId, ct); else Admin();
        var users = await queries.UsersAsync(R.TenantName, filter, levelId, page, ct); var result = new List<UserDto>();
        foreach (var user in users.Content) result.Add(await MapToDtoV2Async(user, ct));
        return new(result, users.TotalElements, users.CurrentPage, users.PageSize);
    }
    public Task<long> CountActiveUsersAsync(long planId, CancellationToken ct = default) => queries.ActiveUserCountAsync(planId, null, ct);
    public async Task<long> CountActiveUsersByPlanIdLevelIdAsync(long planId, long levelId, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await queries.ActiveUserCountAsync(planId, levelId, ct); }
    public async Task<UserDto> GetUserDetailsAsync(string accountName, CancellationToken ct = default)
    {
        const string suffix = "_IRISA_LDAP";
        var dto = await queries.UserDetailsAsync(accountName, ct);
        if (dto == null && accountName.EndsWith(suffix, StringComparison.Ordinal)) dto = await queries.UserDetailsAsync(accountName[..^suffix.Length], ct);
        var current = await FindByIdOrElseThrowAsync(R.User.Id, ct);
        if (dto == null || !string.Equals(current.UserName, dto.UserName, StringComparison.Ordinal)) throw new UnauthorizedAccessException();
        dto.Roles = await queries.UserRolesAsync(current.UserId, ct); return dto;
    }
    public Task<bool> UserIsAdminAsync(long id, CancellationToken ct = default) => queries.HasRoleAsync(id, "Administrators", ct);
    public Task<bool> UserIsRaterAsync(long id, CancellationToken ct = default) => queries.HasRoleAsync(id, "Rater", ct);
    public Task<bool> UserIsMasterRaterAsync(long id, CancellationToken ct = default) => queries.HasRoleAsync(id, "MasterRater", ct);
    public Task<bool> UserIsModirAsync(long id, CancellationToken ct = default) => queries.HasRoleAsync(id, "Modiriat", ct);
    public Task<UserDto> CreateUserAsync(UserDto dto, CancellationToken ct = default) => CreateAsync(dto, ct);
    // SSO provisioning must run under an explicitly authorized provisioning identity (admin here).
    public Task<UserDto> CreateUserFromSSOAsync(UserDto dto, CancellationToken ct = default) => CreateAsync(dto, ct);
    private Task<UserDto> CreateAsync(UserDto dto, CancellationToken ct) => R.TransactionAsync(async () =>
    {
        Admin(); Validate(dto); await CheckLevelsAsync(dto, ct);
        if (string.IsNullOrWhiteSpace(dto.UserName)) throw new BusinessException("نام کاربری الزامی است.");
        if (!string.IsNullOrEmpty(dto.NationalCode) && await GetRepository().AnyAsync(x => x.NationalCode == dto.NationalCode, ct)) throw new BusinessException("این کد ملی قبلا ثبت شده است.");
        if (await GetRepository().AnyAsync(x => x.UserName == dto.UserName, ct)) throw new BusinessException("این نام کاربری قبلا ثبت شده است.");
        var user = new User { UserName = dto.UserName }; await ApplyAsync(user, dto, ct); await SaveAsync(user, ct);
        await external.QueueNewUserNotificationAsync(user, ct); return MapToDto(user);
    }, ct);
    public Task DeleteAsync(long id, CancellationToken ct = default) => R.TransactionAsync(async () =>
    { Admin(); var user = await FindByIdOrElseThrowAsync(id, ct); await external.DeleteUserAccountsRolesMembershipsAndTokensAsync(id, ct); await base.DeleteAsync(user, ct); }, ct);
    public new Task DeleteByIdAsync(long id, CancellationToken ct = default) => DeleteAsync(id, ct);
    public async Task<bool> LoginDisabledAsync(string name, CancellationToken ct = default) => (await FindByUserNameAsync(name, ct))?.LoginDisabled == true;
    public async Task<DateTime> LoginDisabledTimeAsync(string name, CancellationToken ct = default)
    {
        var user = await FindByUserNameAsync(name, ct) ?? throw new UnauthorizedAccessException();
        return user.LoginDisabledTime ?? R.Clock.GetLocalNow().DateTime.AddYears(-1);
    }
    public Task<ServicePage<UserDto>> GetAllTashilgarAsync(string? filter, ServicePageRequest page, CancellationToken ct = default) => queries.UsersByRoleAsync("Tashilgar", filter, page, ct);
    public Task<ServicePage<UserDto>> GetAllModirAsync(string? filter, ServicePageRequest page, CancellationToken ct = default) => queries.UsersByRoleAsync("Modiriat", filter, page, ct);
    public Task DeactivateUserAsync(long id, CancellationToken ct = default) => R.TransactionAsync(async () =>
    { Admin(); await FindByIdOrElseThrowAsync(id, ct); await external.RevokeUserTokensAsync(id, ct); }, ct);
    public Task UnlockUserByUserIdAsync(long id, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        Admin(); var user = await FindByIdOrElseThrowAsync(id, ct);
        if (user.LoginDisabled == true)
        {
            user.LoginDisabled = false; user.LoginDisabledTime = null; user.LastSuccessfulLoginAttempt = null;
            user.LastFailedLoginAttempt = null; user.FailedLoginAttempts = 0; await SaveAsync(user, ct);
        }
    }, ct);
    public async Task SynchronizeWithActiveDirectoryAsync(CancellationToken ct = default)
    {
        Admin();
        var directory = await external.ReadDirectoryUsersAsync(["cn", "sn", "givenName", "name", "sAMAccountName", "department", "description", "mobile", "title", "manager"], 10_000, ct);
        await R.TransactionAsync(async () =>
        {
            var names = new HashSet<string>(await GetRepository().Where(x => x.UserName != null).Select(x => x.UserName).ToListAsync(ct), StringComparer.OrdinalIgnoreCase);
            var inserted = new List<User>();
            foreach (var user in directory)
            {
                if (string.IsNullOrWhiteSpace(user.UserName) || user.UserName.Length > 50 || user.NameF?.Length > 50 || user.FamilyF?.Length > 50 || user.CellPhoneNo?.Length > 50 || !names.Add(user.UserName)) continue;
                inserted.Add(user); GetRepository().Add(user);
            }
            await R.Db.SaveChangesAsync(ct); await external.EnsureLdapAccountsAsync(inserted, ct);
        }, ct);
    }
}
