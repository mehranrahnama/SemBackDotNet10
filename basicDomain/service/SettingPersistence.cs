using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.service.impl;

internal sealed class SettingPersistence(SemDbContext db, TenantContext tenant) : ISettingRepository, ITenantService
{
    private IQueryable<Setting> Settings => db.Set<Setting>().Where(item => item.TenantId == tenant.TenantId);

    public Task<Setting?> FindByKeyAndTenantIdAsync(string key, long tenantId, CancellationToken ct = default) =>
        Settings.SingleOrDefaultAsync(item => item.TenantId == tenantId && item.SettingKey == key, ct);

    public async Task<IReadOnlyList<SettingDto>> FindAllByTenantIdAsync(long tenantId, CancellationToken ct = default) =>
        await Settings.Where(item => item.TenantId == tenantId)
            .Select(item => new SettingDto(item.SettingId, item.TenantId, item.SettingKey, item.SettingValue)).ToListAsync(ct);

    public Task<Setting?> FindByIdAsync(long settingId, CancellationToken ct = default) =>
        Settings.SingleOrDefaultAsync(item => item.SettingId == settingId, ct);

    public async Task<Setting> SaveAsync(Setting setting, CancellationToken ct = default)
    {
        if (setting.TenantId != tenant.TenantId) throw new UnauthorizedAccessException();
        if (db.Entry(setting).State == EntityState.Detached) db.Add(setting);
        await db.SaveChangesAsync(ct);
        return setting;
    }

    public async Task<bool> DeleteByIdAsync(long settingId, CancellationToken ct = default)
    {
        var setting = await FindByIdAsync(settingId, ct);
        if (setting is null) return false;
        db.Remove(setting);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<Tenant> FindByIdOrElseThrowAsync(long tenantId, CancellationToken ct = default)
    {
        if (tenantId != tenant.TenantId) throw new UnauthorizedAccessException();
        var result = await db.Set<SemBackDotNet10.basicDomain.Model.Entity.Tenant>().AsNoTracking()
            .Where(item => item.Id == tenantId).Select(item => new Tenant { Id = item.Id, Name = item.TenantName }).SingleOrDefaultAsync(ct);
        return result ?? throw new ServiceException("Tenant not found.");
    }
}
