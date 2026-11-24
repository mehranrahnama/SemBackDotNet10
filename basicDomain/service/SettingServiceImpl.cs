using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SemBackDotNet10.service.impl;

public sealed class Tenant { public long Id { get; set; } public string? Name { get; set; } }
public sealed class Setting { public long SettingId { get; set; } public long TenantId { get; set; } public string SettingKey { get; set; } = ""; public string? SettingValue { get; set; } }
public sealed record SettingDto(long SettingId, long TenantId, string SettingKey, string? SettingValue);
public interface ITenantService { Task<Tenant> FindByIdOrElseThrowAsync(long tenantId, CancellationToken ct = default); }
public interface ISettingRepository
{
    Task<Setting?> FindByKeyAndTenantIdAsync(string key, long tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<SettingDto>> FindAllByTenantIdAsync(long tenantId, CancellationToken ct = default);
    Task<Setting?> FindByIdAsync(long settingId, CancellationToken ct = default);
    Task<Setting> SaveAsync(Setting setting, CancellationToken ct = default);
    Task<bool> DeleteByIdAsync(long settingId, CancellationToken ct = default);
}

/// <summary>Tenant-scoped setting operations. Depends on ISettingRepository and ITenantService.</summary>
public sealed class SettingServiceImpl(ISettingRepository settingRepository, ITenantService tenantService, string tenantName, IServiceLogger? logger = null)
{
    private readonly IServiceLogger _logger = logger ?? new NullServiceLogger();
    public string GetTenantName() => tenantName;
    public async Task<Setting> CreateSettingAsync(long tenantId, string key, string? value, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        if (key.Equals("max.expired.date", StringComparison.OrdinalIgnoreCase)) throw new ServiceException("این پیکربندی قبلا ثبت شده است");
        var current = await settingRepository.FindByKeyAndTenantIdAsync(key, tenantId, ct);
        if (current is not null) { current.SettingValue = string.Concat(current.SettingValue, ",", value); return await settingRepository.SaveAsync(current, ct); }
        var tenant = await tenantService.FindByIdOrElseThrowAsync(tenantId, ct);
        return await settingRepository.SaveAsync(new Setting { TenantId = tenant.Id, SettingKey = key, SettingValue = value }, ct);
    }
    public Task<IReadOnlyList<SettingDto>> GetAllByTenantIdAsync(long tenantId, CancellationToken ct = default) => settingRepository.FindAllByTenantIdAsync(tenantId, ct);
    public async Task<Setting> EditSettingAsync(Setting setting, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(setting);
        var found = await settingRepository.FindByIdAsync(setting.SettingId, ct) ?? throw new ServiceException("Setting not found.");
        var tenant = await tenantService.FindByIdOrElseThrowAsync(setting.TenantId, ct);
        found.SettingKey = setting.SettingKey; found.SettingValue = setting.SettingValue; found.TenantId = tenant.Id;
        return await settingRepository.SaveAsync(found, ct);
    }
    public async Task<bool> RemoveSettingAsync(long settingId, CancellationToken ct = default)
    { if (await settingRepository.DeleteByIdAsync(settingId, ct)) return true; throw new ServiceException("چنین پیکربندی وجود ندارد"); }
    public async Task<string?> GetSettingByKeyAndTenantIdAsync(long tenantId, string key, CancellationToken ct = default)
        => (await settingRepository.FindByKeyAndTenantIdAsync(key, tenantId, ct))?.SettingValue;
}
