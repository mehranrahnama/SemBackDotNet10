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

public interface IPorterScoreRepository : IBaseRepository<PorterScore>
{
    Task<PagedResult<PorterScoreDto>> FindByPorterGroupIdAsync(long? porterGroupId, PageRequest pageRequest, CancellationToken ct = default);

    Task<List<PorterScoreDto>> FindByPorterGroupIdAsync(long? porterGroupId, CancellationToken ct = default);

    Task<List<PorterScoreDto>> GetKeyOpportunityByPorterGroupAsync(long? porterGroupId, long? planId, CancellationToken ct = default);

    Task<List<PorterScoreDto>> GetKeyThreatByPorterGroupAsync(long? porterGroupId, long? planId, CancellationToken ct = default);

    Task<List<PorterScoreDto>> GetAllByPorterGroupAndProductAndMarketAsync(long? porterGroupId, long? marketId, long? productId, CancellationToken ct = default);
}

public class PorterScoreRepository : BaseRepository<PorterScore>, IPorterScoreRepository
{
    public PorterScoreRepository(SemDbContext context) : base(context) { }

    private static IQueryable<PorterScoreDto> Project(IQueryable<PorterScore> source) =>
        source.Select(ps => new PorterScoreDto(
            ps.Id,
            ps.PorterGroup.Id,
            ps.PorterGroup.Title,
            ps.PlanId,
            ps.Title,
            ps.CurrentScore,
            ps.FutureScore,
            ps.Opportunity));

    public async Task<PagedResult<PorterScoreDto>> FindByPorterGroupIdAsync(long? porterGroupId, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Project(Query.Where(ps => porterGroupId != null && ps.PorterGroupId == porterGroupId));
        var total = await query.LongCountAsync(ct);
        var items = await global::Common.utils.PageUtils.Apply(query, pageRequest).ToListAsync(ct);
        return new PagedResult<PorterScoreDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public Task<List<PorterScoreDto>> FindByPorterGroupIdAsync(long? porterGroupId, CancellationToken ct = default) =>
        Project(Query.Where(ps => porterGroupId != null && ps.PorterGroupId == porterGroupId)).ToListAsync(ct);

    public Task<List<PorterScoreDto>> GetKeyOpportunityByPorterGroupAsync(long? porterGroupId, long? planId, CancellationToken ct = default) =>
        Project(Query.Where(ps => porterGroupId != null && ps.PorterGroupId == porterGroupId
                                  && planId != null && ps.PlanId == planId
                                  && (ps.CurrentScore == 4 || ps.CurrentScore == 5 || ps.FutureScore == 4 || ps.FutureScore == 5)))
            .ToListAsync(ct);

    public Task<List<PorterScoreDto>> GetKeyThreatByPorterGroupAsync(long? porterGroupId, long? planId, CancellationToken ct = default) =>
        Project(Query.Where(ps => porterGroupId != null && ps.PorterGroupId == porterGroupId
                                  && planId != null && ps.PlanId == planId
                                  && (ps.CurrentScore == -4 || ps.CurrentScore == -5 || ps.FutureScore == -4 || ps.FutureScore == -5)))
            .ToListAsync(ct);

    public Task<List<PorterScoreDto>> GetAllByPorterGroupAndProductAndMarketAsync(long? porterGroupId, long? marketId, long? productId, CancellationToken ct = default) =>
        Project(Query.Where(ps => porterGroupId != null && ps.PorterGroupId == porterGroupId
                                  && marketId != null && ps.MarketId == marketId
                                  && productId != null && ps.ProductId == productId))
            .ToListAsync(ct);
}
