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

public interface IMarketRepository : IBaseRepository<Market>
{
    Task<PagedResult<MarketDto>> FindAllPageAsync(string? filter, long? planId, PageRequest pageRequest, CancellationToken ct = default);
}

public class MarketRepository : BaseRepository<Market>, IMarketRepository
{
    public MarketRepository(SemDbContext context) : base(context) { }

    public async Task<PagedResult<MarketDto>> FindAllPageAsync(string? filter, long? planId, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Query
            .Where(m => planId != null && m.PlanId == planId && (filter == null || m.Title.Contains(filter)))
            .Select(m => new MarketDto(m.Id, m.Title, m.PlanId));

        var total = await query.LongCountAsync(ct);
        var items = await global::Common.utils.PageUtils.Apply(query, pageRequest).ToListAsync(ct);
        return new PagedResult<MarketDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }
}
