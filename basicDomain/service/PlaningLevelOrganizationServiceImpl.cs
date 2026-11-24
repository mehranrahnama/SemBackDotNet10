#nullable enable
using Microsoft.EntityFrameworkCore;
namespace SemBackDotNet10.service.impl;

public sealed class PlaningLevelOrganizationServiceImpl(ServiceRuntime runtime) : ServiceBase<PlaningLevelOrganization>(runtime)
{
    public Task<ServicePage<PlaningLevelOrganizationDto>> FindAllAsync(string filter, ServicePageRequest page, CancellationToken ct = default)
        => R.Queries.LevelOrganizationsAsync(ServiceJson.Filter(filter), null, false, page, ct);
    public async Task<ServicePage<PlaningLevelOrganizationDto>> FindPlaningLevelOrganizationsByLevelIDAsync(string body, ServicePageRequest page, CancellationToken ct = default)
    {
        var id = ServiceJson.NullableLong(body, "levelId"); await AccessAsync(id, ct);
        return await R.Queries.LevelOrganizationsAsync(ServiceJson.Filter(body), id, true, page, ct);
    }
    public PlaningLevelOrganizationDto MapToDto(PlaningLevelOrganization entity) => new()
    { Id = entity.Id, LevelId = entity.Level?.Id, OrganizationId = entity.Organization?.Id, DummyField = entity.DummyField };
    public async Task<PlaningLevelOrganization> MapToEntityAsync(PlaningLevelOrganizationDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var entity = new PlaningLevelOrganization { Id = dto.Id, DummyField = dto.DummyField };
        if (dto.LevelId is not null) entity.Level = await RequireAsync<PlaningLevel>(dto.LevelId, "error.PlaningLevel.idIsNotValid", ct);
        if (dto.OrganizationId is not null) entity.Organization = await RequireAsync<Organization>(dto.OrganizationId, "error.organization.idIsNotValid", ct);
        return entity;
    }
    public Task<PlaningLevelOrganization> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
        => RequireAsync<PlaningLevelOrganization>(id, "error.planingLevelOrganization.idIsNotValid", ct);
    public Task<PlaningLevelOrganization> CreateAsync(PlaningLevelOrganizationDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            return await SaveAsync(await MapToEntityAsync(dto, ct), ct);
        }, ct);
    public Task<PlaningLevelOrganization> EditAsync(PlaningLevelOrganizationDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            var entity = await FindByIdOrElseThrowAsync(dto.Id, ct);
            entity.DummyField = dto.DummyField;
            if (dto.LevelId is not null) await AccessAsync(dto.LevelId, ct);
            if (dto.LevelId is not null) entity.Level = await RequireAsync<PlaningLevel>(dto.LevelId, "error.PlaningLevel.idIsNotValid", ct);
            if (dto.OrganizationId is not null) entity.Organization = await RequireAsync<Organization>(dto.OrganizationId, "error.organization.idIsNotValid", ct);
            return await SaveAsync(entity, ct);
        }, ct);
    public Task DeleteAsync(long? id, CancellationToken ct = default)
        => R.TransactionAsync(async () => await base.DeleteAsync(await FindByIdOrElseThrowAsync(id, ct), ct), ct);
}
