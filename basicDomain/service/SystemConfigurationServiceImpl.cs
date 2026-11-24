#nullable enable
using SemBackDotNet10.Models.Entities;
using ConfigurationTenant = SemBackDotNet10.basicDomain.Model.Entity.Tenant;
using System.Text.Json;

namespace SemBackDotNet10.service.impl;

// Implement using an EF projection and parameterized filtering on the stored enum name.
// Must apply the same tenant restrictions as the application's other repositories.
public interface ISystemConfigurationQueries
{
    Task<ServicePage<SystemConfigurationDto>> FindAllAsync(string keyword, ServicePageRequest page, CancellationToken ct);
}

public sealed class SystemConfigurationServiceImpl(ServiceRuntime runtime, ISystemConfigurationQueries queries)
    : ServiceBase<SystemConfiguration>(runtime)
{
    public Task<ServicePage<SystemConfigurationDto>> FindAllAsync(string filter, ServicePageRequest page, CancellationToken ct = default)
    {
        using var json = JsonDocument.Parse(filter);
        var keyword = json.RootElement.TryGetProperty("filter", out var value)
            ? value.GetString() ?? throw new JsonException("filter must be a string") : "";
        return queries.FindAllAsync(keyword, page, ct);
    }

    public SystemConfigurationDto MapToDto(SystemConfiguration entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SystemConfigurationDto
        {
            Id = entity.Id,
            MultiTargetActionPlan = entity.MultiTargetActionPlan,
            MeasureTrend = entity.MeasureTrend?.ToString(),
            TenantId = entity.TenantId ?? entity.Tenant?.Id
        };
    }

    private static MeasureTrend ParseTrend(string? value)
    {
        if (!Enum.GetNames<MeasureTrend>().Contains(value, StringComparer.Ordinal))
            throw new ArgumentException("Unknown MeasureTrend name", nameof(value));
        return Enum.Parse<MeasureTrend>(value!, ignoreCase: false);
    }

    public async Task<SystemConfiguration> MapToEntityAsync(SystemConfigurationDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var entity = new SystemConfiguration { MultiTargetActionPlan = dto.MultiTargetActionPlan };
        if (dto.Id is long id) entity.Id = id;
        entity.MeasureTrend = ParseTrend(dto.MeasureTrend);
        if (dto.TenantId is not null)
            entity.Tenant = await RequireAsync<ConfigurationTenant>(dto.TenantId, "error.tenant.idIsNotValid", ct);
        return entity;
    }

    public Task<SystemConfiguration> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
        => RequireAsync<SystemConfiguration>(id, "error.systemConfiguration.idIsNotValid", ct);

    public Task<SystemConfiguration> CreateAsync(SystemConfigurationDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () => await SaveAsync(await MapToEntityAsync(dto, ct), ct), ct);

    public Task<SystemConfiguration> EditAsync(SystemConfigurationDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            var entity = await FindByIdOrElseThrowAsync(dto.Id, ct);
            entity.MeasureTrend = ParseTrend(dto.MeasureTrend);
            entity.MultiTargetActionPlan = dto.MultiTargetActionPlan;
            // Java preserves the previous tenant when TenantId is null.
            if (dto.TenantId is not null)
                entity.Tenant = await RequireAsync<ConfigurationTenant>(dto.TenantId, "error.tenant.idIsNotValid", ct);
            return await SaveAsync(entity, ct);
        }, ct);

    public Task DeleteAsync(long? id, CancellationToken ct = default)
        => R.TransactionAsync(async () => await base.DeleteAsync(await FindByIdOrElseThrowAsync(id, ct), ct), ct);
}
