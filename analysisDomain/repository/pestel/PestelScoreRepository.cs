using global::SemBackDotNet10.Common.Pagination;
using Common.utils;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.model.entity.pestel;

using SemBackDotNet10.Data;
using System.Linq.Dynamic.Core;

namespace SemBackDotNet10.analysisDomain.repository.pestel;

public interface IPestelScoreRepository : IBaseRepository<PestelScore>
{
    Task<global::SemBackDotNet10.Common.Pagination.PagedResult<PestelScoreDto>> FindByPestelGroupIdAsync(long? pestelGroupId, PageRequest pageRequest, CancellationToken ct = default);

    Task<List<PestelScoreDto>> GetKeyOpportunityByPestelGroupAsync(long? pestelGroupId, long? planId, CancellationToken ct = default);

    Task<List<PestelScoreDto>> GetKeyThreatByPestelGroupAsync(long? pestelGroupId, long? planId, CancellationToken ct = default);
}

public class PestelScoreRepository : BaseRepository<PestelScore>, IPestelScoreRepository
{
    public PestelScoreRepository(SemDbContext context) : base(context) { }

    private static IQueryable<PestelScoreDto> Project(IQueryable<PestelScore> source) =>
        source.Select(ps => new PestelScoreDto
        {
            Id = ps.Id,
            PestelDto = new PestelDto
            {
                Id = ps.Pestel.Id,
                PestelGroupId = ps.Pestel.PestelGroup.Id,
                PestelGroupTitle = ps.Pestel.PestelGroup.Title,
                Title = ps.Pestel.Title
            },
            PlanId = ps.PlanId,
            OpportunityOrThreat = ps.OpportunityOrThreat,
            CurrentScore = ps.CurrentScore,
            FutureScore = ps.FutureScore,
            PreviousYearCurrentScore = ps.PreviousYearCurrentScore,
            PreviousYearFutureScore = ps.PreviousYearFutureScore
        });

    public async Task<global::SemBackDotNet10.Common.Pagination.PagedResult<PestelScoreDto>> FindByPestelGroupIdAsync(long? pestelGroupId, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Project(Query
            .Where(ps => ps.Pestel.PestelGroup.Id == pestelGroupId)
            .OrderBy(ps => ps.Id));

        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageRequest).ToListAsync(ct);
        return new global::SemBackDotNet10.Common.Pagination.PagedResult<PestelScoreDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public Task<List<PestelScoreDto>> GetKeyOpportunityByPestelGroupAsync(long? pestelGroupId, long? planId, CancellationToken ct = default) =>
        Project(Query.Where(ps => ps.Pestel.PestelGroup.Id == pestelGroupId
                                  && planId != null && ps.Pestel.PestelGroup.PlanId == planId
                                  && (ps.CurrentScore == 4 || ps.CurrentScore == 5 || ps.FutureScore == 4 || ps.FutureScore == 5)))
            .ToListAsync(ct);

    public Task<List<PestelScoreDto>> GetKeyThreatByPestelGroupAsync(long? pestelGroupId, long? planId, CancellationToken ct = default) =>
        Project(Query.Where(ps => ps.Pestel.PestelGroup.Id == pestelGroupId
                                  && planId != null && ps.Pestel.PestelGroup.PlanId == planId
                                  && (ps.CurrentScore == -4 || ps.CurrentScore == -5 || ps.FutureScore == -4 || ps.FutureScore == -5)))
            .ToListAsync(ct);
}
