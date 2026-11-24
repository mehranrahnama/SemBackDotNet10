using global::SemBackDotNet10.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Dto;
using SemBackDotNet10.basicDomain.Model.Entity;
using static global::Common.utils.PageUtils;
using SemBackDotNet10.Common.utils;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.basicDomain.Repositories;

public interface IOrganizationRepository : IBaseRepository<Organization>
{
    Task<Organization?> FindTopByOrderByIdDescAsync(CancellationToken ct = default);

    Task<PagedResult<OrganizationDto>> SearchByDesAsync(string keyword, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default);

    Task<PagedResult<OrganizationDto>> CommitteeAllWithModirInfoAsync(string? keyword, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default);

    Task<PagedResult<OrganizationDto>> OrganizationAllWithModirInfoAsync(string? keyword, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default);
}

public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(SemDbContext context) : base(context) { }

    public Task<Organization?> FindTopByOrderByIdDescAsync(CancellationToken ct = default) =>
        Context.Organizations.OrderByDescending(o => o.Id).FirstOrDefaultAsync(ct);

    public async Task<PagedResult<OrganizationDto>> SearchByDesAsync(string keyword, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Context.Organizations
            .Where(o => keyword == null || o.DesOrganF!.Contains(keyword))
            .Select(o => new OrganizationDto
            {
                Id = o.Id,
                DesOrganF = o.DesOrganF,
                ModirFullName = o.Modir == null
                    ? string.Empty
                    : o.Modir.NameF + " " + o.Modir.FamilyF
            });

        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageRequest).ToListAsync(ct);
        return new PagedResult<OrganizationDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public async Task<PagedResult<OrganizationDto>> CommitteeAllWithModirInfoAsync(string? keyword, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Context.PlaningLevels.AsNoTracking()
            .Where(level => keyword == null || level.DesLevel!.Contains(keyword))
            .Select(level => new OrganizationDto
            {
                Id = level.Id, DesLevel = level.DesLevel,
                ModirId = level.FacilitatorId,
                ModirFullName = level.Facilitator == null ? " " :
                    (level.Facilitator.NameF ?? "") + " " + (level.Facilitator.FamilyF ?? "")
            });
        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageRequest).ToListAsync(ct);
        return new PagedResult<OrganizationDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public async Task<PagedResult<OrganizationDto>> OrganizationAllWithModirInfoAsync(string? keyword, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Context.PlaningLevelOrganizations.AsNoTracking()
            .Where(link => link.Organization != null && link.Organization.ModirId != null
                && (keyword == null || link.Organization.DesOrganF!.Contains(keyword)))
            .Select(link => new OrganizationDto
            {
                Id = link.Level!.Id, DesLevel = link.Level.DesLevel,
                OrganId = link.Organization!.Id, DesOrganF = link.Organization.DesOrganF,
                ModirId = link.Organization.ModirId,
                ModirFullName = link.Organization.Modir == null ? " " :
                    (link.Organization.Modir.NameF ?? "") + " " + (link.Organization.Modir.FamilyF ?? "")
            });
        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageRequest).ToListAsync(ct);
        return new PagedResult<OrganizationDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }
}
