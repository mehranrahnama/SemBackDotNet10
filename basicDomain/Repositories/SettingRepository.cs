#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Models.Entities;

namespace SemBackDotNet10.baseDomain.repository;

public interface ISettingRepository : ISemRepository<Setting, long>
{
    Task<Setting?> FindBySettingKeyAndTenant_IdAsync(string key, long tenantId, CancellationToken ct = default);
    Task<List<SettingDTO>> FindAllByTenant_IdAsync(long tenantId, CancellationToken ct = default);
}

public sealed class SettingRepository(DbContext db)
    : SemRepository<Setting, long>(db), ISettingRepository
{
    public Task<Setting?> FindBySettingKeyAndTenant_IdAsync(string key, long tenantId, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.SettingKey == key && x.TenantId == tenantId, ct);
    }

    public Task<List<SettingDTO>> FindAllByTenant_IdAsync(long tenantId, CancellationToken ct = default)
    {
        return Set.Where(x => x.TenantId == tenantId).Select(x => new SettingDTO(x.Id, x.SettingKey, x.SettingValue, x.TenantId)).ToListAsync(ct);
    }
}
