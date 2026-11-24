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

public interface IProductGroupRepository : IBaseRepository<ProductGroup>
{
    Task<PagedResult<ProductGroupDto>> FindAllPageAsync(string? filter, long? planId, PageRequest pageRequest, CancellationToken ct = default);

    Task<List<ProductGroupDto>> FindAllByPlanPeriodPlanIdAsync(long? planId, CancellationToken ct = default);
}

public class ProductGroupRepository : BaseRepository<ProductGroup>, IProductGroupRepository
{
    public ProductGroupRepository(SemDbContext context) : base(context) { }

    public async Task<PagedResult<ProductGroupDto>> FindAllPageAsync(string? filter, long? planId, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Query
            .Where(pg => planId != null && pg.PlanId == planId && (filter == null || pg.Title.Contains(filter)))
            .Select(pg => new ProductGroupDto(pg.Id, pg.Title, pg.PlanId));

        var total = await query.LongCountAsync(ct);
        var items = await global::Common.utils.PageUtils.Apply(query, pageRequest).ToListAsync(ct);
        return new PagedResult<ProductGroupDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public Task<List<ProductGroupDto>> FindAllByPlanPeriodPlanIdAsync(long? planId, CancellationToken ct = default) =>
        Query
            .Where(pg => planId != null && pg.PlanId == planId)
            .Select(pg => new ProductGroupDto(pg.Id, pg.Title, pg.PlanId))
            .ToListAsync(ct);
}
