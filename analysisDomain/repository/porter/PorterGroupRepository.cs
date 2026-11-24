using PageRequest = global::Common.utils.PageRequest;
using global::SemBackDotNet10.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.basicDomain.Repositories;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.analysisDomain.repository.porter;

public interface IPorterGroupRepository : IBaseRepository<PorterGroup>
{
    Task<PagedResult<PorterGroupDto>> FindAllPageAsync(string? filter, long? planId, PageRequest pageRequest, CancellationToken ct = default);

    Task<List<PorterGroup>> FindAllByPlanIdAsync(long? planId, CancellationToken ct = default);
}

public class PorterGroupRepository : BaseRepository<PorterGroup>, IPorterGroupRepository
{
    public PorterGroupRepository(SemDbContext context) : base(context) { }

    public async Task<PagedResult<PorterGroupDto>> FindAllPageAsync(string? filter, long? planId, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Query
            .Where(pg => planId != null && pg.PlanId == planId && (filter == null || pg.Title.Contains(filter)))
            .Select(pg => new PorterGroupDto(pg.Id, pg.Title, pg.PlanId));

        var total = await query.LongCountAsync(ct);
        var items = await global::Common.utils.PageUtils.Apply(query, pageRequest).ToListAsync(ct);
        return new PagedResult<PorterGroupDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public Task<List<PorterGroup>> FindAllByPlanIdAsync(long? planId, CancellationToken ct = default) =>
        Query.Where(pg => planId != null && pg.PlanId == planId).ToListAsync(ct);
}
