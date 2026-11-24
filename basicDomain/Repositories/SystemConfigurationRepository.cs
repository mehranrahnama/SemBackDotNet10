#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Models.Entities;


namespace SemBackDotNet10.baseDomain.repository;

public interface ISystemConfigurationRepository : ISemRepository<SystemConfiguration, long>
{
    Task<PageResult<SystemConfigurationDto>> FindAllSystemConfigurationsAsync(PageRequest page, string? filter, CancellationToken ct = default);
}

public sealed class SystemConfigurationRepository(DbContext db)
    : SemRepository<SystemConfiguration, long>(db), ISystemConfigurationRepository
{
    public Task<PageResult<SystemConfigurationDto>> FindAllSystemConfigurationsAsync(PageRequest page, string? filter, CancellationToken ct = default)
    {
        return PageAsync(Set.Where(x => filter == null || EF.Functions.Like(x.MeasureTrend.ToString(), "%" + filter + "%")).OrderBy(x => x.Id).Select(x => new SystemConfigurationDto(x.Id, x.Tenant.Id, x.MultiTargetActionPlan, x.MeasureTrend.ToString())), page, ct);
    }
}
