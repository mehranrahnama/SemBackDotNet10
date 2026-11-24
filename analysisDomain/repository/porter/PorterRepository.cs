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

public interface IPorterRepository : IBaseRepository<Porter>
{
    Task<PagedResult<PorterDto>> FindAllPageAsync(long porterGroupId, string? filter, PageRequest pageRequest, CancellationToken ct = default);

    Task<List<PorterDto>> FindAllAsync(long porterGroupId, string? filter, CancellationToken ct = default);

    Task<List<Porter>> FindAllByPorterGroupIdAsync(long porterGroupId, CancellationToken ct = default);
}

public class PorterRepository : BaseRepository<Porter>, IPorterRepository
{
    public PorterRepository(SemDbContext context) : base(context) { }

    private IQueryable<PorterDto> ProjectedQuery(long porterGroupId, string? filter) =>
        Query
            .Where(p => p.PorterGroupId == porterGroupId && (filter == null || p.Title.Contains(filter)))
            .Select(p => new PorterDto(p.Id, p.Title, p.NoThreat, p.MiddleThreat, p.SeriousThreat, p.PorterGroup.Id, p.PorterGroup.Title));

    public async Task<PagedResult<PorterDto>> FindAllPageAsync(long porterGroupId, string? filter, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = ProjectedQuery(porterGroupId, filter);
        var total = await query.LongCountAsync(ct);
        var items = await global::Common.utils.PageUtils.Apply(query, pageRequest).ToListAsync(ct);
        return new PagedResult<PorterDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public Task<List<PorterDto>> FindAllAsync(long porterGroupId, string? filter, CancellationToken ct = default) =>
        ProjectedQuery(porterGroupId, filter).ToListAsync(ct);

    public Task<List<Porter>> FindAllByPorterGroupIdAsync(long porterGroupId, CancellationToken ct = default) =>
        Query.Where(p => p.PorterGroupId == porterGroupId).ToListAsync(ct);
}
