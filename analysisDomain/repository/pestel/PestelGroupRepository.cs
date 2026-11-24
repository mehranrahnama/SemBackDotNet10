using PageRequest = global::Common.utils.PageRequest;
using global::SemBackDotNet10.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.model.entity.pestel;
using SemBackDotNet10.basicDomain.Repositories;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.analysisDomain.repository.pestel;

public interface IPestelGroupRepository : IBaseRepository<PestelGroup>
{
    Task<PagedResult<PestelGroupDto>> FindAllPageAsync(string? title, long? planId, PageRequest pageRequest, CancellationToken ct = default);

    Task<List<PestelGroup>> FindAllByPlanIdAsync(long? planId, CancellationToken ct = default);
}

public class PestelGroupRepository : BaseRepository<PestelGroup>, IPestelGroupRepository
{
    public PestelGroupRepository(SemDbContext context) : base(context) { }

    public async Task<PagedResult<PestelGroupDto>> FindAllPageAsync(string? title, long? planId, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Query
            .Where(pg => planId != null && pg.PlanId == planId)
            .Where(pg => title == null || pg.Title.Contains(title))
            .OrderBy(pg => pg.Id)
            .Select(pg => new PestelGroupDto
            {
                Id = pg.Id,
                Title = pg.Title,
                PlanId = pg.PlanId
            });

        var total = await query.LongCountAsync(ct);
        var items = await global::Common.utils.PageUtils.Apply(query, pageRequest).ToListAsync(ct);
        return new PagedResult<PestelGroupDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public Task<List<PestelGroup>> FindAllByPlanIdAsync(long? planId, CancellationToken ct = default) =>
        Query.Where(pg => planId != null && pg.PlanId == planId).ToListAsync(ct);
}
