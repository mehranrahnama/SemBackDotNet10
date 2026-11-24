#nullable enable
using Common.exception;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.basicDomain.service.impl;

// User membership queries must use real account/role joins, not just tenant existence.
public interface ITenantMembershipQueries
{
    Task<User> FindUserByUserNameAsync(string username, CancellationToken ct);
    Task<Tenant?> FindUserDefaultTenantAsync(long userId, CancellationToken ct);
    Task<Tenant?> FindByIdAndUserNameAsync(long tenantId, string username, CancellationToken ct);
}
public sealed class TenantServiceImpl(ServiceRuntime runtime, ITenantMembershipQueries membership) : ServiceBase<Tenant>(runtime), ITenantService
{
    public Task<Tenant> FindByIdOrElseThrowAsync(long id, CancellationToken ct = default)
        => FindByIdOrElseThrowAsync((long?)id, ct);

    public Task<Tenant> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
        => RequireAsync<Tenant>(id, "error.tenant.idIsNotValid", ct);
    public Task<Tenant> GetUserPrivateTenantObjAsync(UserAccount account, CancellationToken ct = default)
        => R.TransactionAsync(async () => await SaveAsync(new Tenant { TenantName = account.AccountName + "_WorkSpace" }, ct), ct);
    public Task DeleteTenantAsync(long? id, CancellationToken ct = default)
        => R.TransactionAsync(async () => await DeleteAsync(await FindByIdOrElseThrowAsync(id, ct), ct), ct);
    public async Task<Tenant> FindByNameOrElseThrowAsync(string name, CancellationToken ct = default)
        => await GetRepository().SingleOrDefaultAsync(x => x.TenantName == name, ct)
            ?? throw new BusinessException("error.tenant.NameNotValid");
    public async Task<Tenant?> GetDefaultUserTenantAsync(string username, CancellationToken ct = default)
    {
        var user = await membership.FindUserByUserNameAsync(username, ct);
        return await membership.FindUserDefaultTenantAsync(user.UserId, ct)
            ?? await GetRepository().SingleOrDefaultAsync(x => x.TenantCode == R.TenantName, ct);
    }
    public async Task<TenantDto> MapToDtoAsync(Tenant entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var id = entity.Id;
        entity = await FindByIdOrElseThrowAsync(id, ct);
        return new TenantDto { Id = entity.Id, TenantName = entity.TenantName, TenantCode = entity.TenantCode, TenantParentId = entity.TenantParentId };
    }
    public async Task<Tenant?> FindByIdAndUserNameAsync(string currentTenant, string username, CancellationToken ct = default)
    {
        if (!long.TryParse(currentTenant, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out var tenantId))
            throw new BusinessException("error.tenant.idIsNotValid");

        var user = await membership.FindUserByUserNameAsync(username, ct);
        if (string.IsNullOrWhiteSpace(user.UserName))
            throw new BusinessException("نام کاربری کاربر مشخص نشده است.");

        return await membership.FindByIdAndUserNameAsync(tenantId, user.UserName, ct)
            ?? await GetRepository().SingleOrDefaultAsync(x => x.TenantCode == R.TenantName, ct);
    }
    Tenant? ITenantService.FindById(long id) => GetRepository().SingleOrDefault(x => x.Id == id);
    Tenant ITenantService.FindByIdOrElseThrow(long id) => FindByIdOrElseThrowAsync(id).GetAwaiter().GetResult();
    Tenant ITenantService.GetUserPrivateTenantObj(UserAccount account) => GetUserPrivateTenantObjAsync(account).GetAwaiter().GetResult();
    void ITenantService.DeleteTenant(long id) => DeleteTenantAsync(id).GetAwaiter().GetResult();
    Tenant ITenantService.FindByNameOrElseThrow(string defaultTenant) => FindByNameOrElseThrowAsync(defaultTenant).GetAwaiter().GetResult();
    Tenant? ITenantService.GetDefaultUserTenant(string username) => GetDefaultUserTenantAsync(username).GetAwaiter().GetResult();
    TenantDto ITenantService.MapToDto(Tenant tenant) => MapToDtoAsync(tenant).GetAwaiter().GetResult();
    Tenant? ITenantService.FindByIdAndUserName(string currentTenant, string username) => FindByIdAndUserNameAsync(currentTenant, username).GetAwaiter().GetResult();

    Tenant ITenantService.FindByCodeOrElseThrow(string defaultTenant)
        => GetRepository().SingleOrDefault(x => x.TenantCode == defaultTenant)
            ?? throw new BusinessException("error.tenant.NameNotValid");

    public async Task<Tenant> FindByCodeOrElseThrowAsync(string code, CancellationToken ct = default)
        => await GetRepository().SingleOrDefaultAsync(x => x.TenantCode == code, ct)
            ?? throw new BusinessException("error.tenant.NameNotValid");
}
