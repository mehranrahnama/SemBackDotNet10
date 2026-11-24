#nullable enable
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.service.impl;

public sealed class OrganizationServiceImpl(ServiceRuntime runtime) : ServiceBase<Organization>(runtime)
{
    public async Task<ServicePage<OrganizationDto>> FindAllAsync(string? search, ServicePageRequest page, CancellationToken ct = default)
    {
        var result = await PageAsync(GetRepository().Include(x => x.Modir), page, ct);
        return new(result.Content.Select(MapToDto).ToList(), result.TotalElements, page.Page, page.Size);
    }
    public OrganizationDto MapToDto(Organization entity) => new()
    { Id = entity.Id, DesOrganF = entity.DesOrganF, ModirFullName = entity.Modir is null ? "" : entity.Modir.NameF + " " + entity.Modir.FamilyF };
    public Task<ServicePage<OrganizationDto>> FindOrganizationsAsync(string filter, ServicePageRequest page, CancellationToken ct = default)
        => R.Queries.OrganizationsAsync(ServiceJson.Filter(filter), page, ct);
    // Java uses MAX(Id)+1. Configure a database sequence for Id instead; MAX+1 is unsafe under concurrency.
    public Task<OrganizationDto> CreateOrganizationAsync(OrganizationDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            var entity = new Organization { DesOrganF = dto.DesOrganF, Modir = await RequireAsync<User>(dto.ModirId, "error.user.idIsNotValid", ct) };
            await SaveAsync(entity, ct); return MapToDto(entity);
        }, ct);
    public Task<Organization> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
        => RequireAsync<Organization>(id, "error.Organization.idIsNotValid", ct);
    public Task EditOrganizationAsync(OrganizationDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            var entity = await FindByIdOrElseThrowAsync(dto.Id, ct);
            entity.DesOrganF = dto.DesOrganF;
            entity.Modir = await RequireAsync<User>(dto.ModirId, "error.user.idIsNotValid", ct);
            await SaveAsync(entity, ct);
        }, ct);
    public Task DeleteOrganizationAsync(long id, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            var links = R.Db.Set<PlaningLevelOrganization>();
            links.RemoveRange(await links.Where(x => x.Organization.Id == id).ToListAsync(ct));
            await R.Db.SaveChangesAsync(ct);
            await DeleteAsync(await FindByIdOrElseThrowAsync(id, ct), ct);
        }, ct);
    public Task<ServicePage<OrganizationDto>> FindAllPositionlistWithModirInfoAsync(string filter, ServicePageRequest page, CancellationToken ct = default)
    {
        using var doc = JsonDocument.Parse(filter); var root = doc.RootElement;
        var committee = root.TryGetProperty("isCommittee", out var value) && value.ValueKind != JsonValueKind.Null && value.GetBoolean();
        return R.Queries.PositionsWithModirAsync(ServiceJson.Text(root, "filter"), committee, page with { OrderBy = "Id", Direction = SortDirection.Ascending }, ct);
    }
}
