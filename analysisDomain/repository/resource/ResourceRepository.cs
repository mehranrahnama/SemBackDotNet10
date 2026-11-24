using global::Common.utils;
using PageRequest = global::Common.utils.PageRequest;
using global::SemBackDotNet10.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.resource;
using SemBackDotNet10.analysisDomain.model.entity.resource;
using SemBackDotNet10.basicDomain.Model.Dto;
using SemBackDotNet10.basicDomain.Repositories;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.analysisDomain.repository.resource;

public interface IResourceRepository : IBaseRepository<Resource>
{
    Task<PagedResult<ResourceDto>> FindAllPageAsync(long levelId, long planId, string? filter, PageRequest pageable, CancellationToken ct = default);

    Task<List<ResourceDto>> FindAllAsync(long levelId, long planId, string? filter, CancellationToken ct = default);

    Task<PagedResult<ResourceDto>> FindKeyResourceByPlanIdAsync(long planId, PageRequest pageable, CancellationToken ct = default);

    Task<List<ResourceDto>> GetKeyResourceByPlanIdListAsync(long planId, CancellationToken ct = default);

    Task<List<ResourceDto>> GetKeyResourceByPlanIdListIrisaAsync(long planId, CancellationToken ct = default);
}

public class ResourceRepository : BaseRepository<Resource>, IResourceRepository
{
    public ResourceRepository(SemDbContext context) : base(context) { }

    private static IQueryable<ResourceDto> ProjectBasic(IQueryable<Resource> query) =>
        query.Select(r => new ResourceDto
        {
            Id = r.Id,
            Title = r.Title,
            Resource = r.IsResource,
            ExistenceStatus = r.ExistenceStatus,
            PlaningLevelDto = new PlaningLevelDto
            {
                Id = r.PlaningLevelId ?? 0,
                DesLevel = r.PlaningLevel != null ? r.PlaningLevel.DesLevel : null
            },
            PlanId = r.PlanId,
            Power = r.Power,
            Importance = r.Importance
        });

    private static IQueryable<ResourceDto> ProjectFull(IQueryable<Resource> query) =>
        query.Select(r => new ResourceDto
        {
            Id = r.Id,
            Title = r.Title,
            Resource = r.IsResource,
            ExistenceStatus = r.ExistenceStatus,
            PlaningLevelDto = new PlaningLevelDto
            {
                Id = r.PlaningLevelId ?? 0,
                DesLevel = r.PlaningLevel != null ? r.PlaningLevel.DesLevel : null
            },
            PlanId = r.PlanId,
            Power = r.Power,
            Importance = r.Importance,
            Valuable = r.Valuable,
            Rare = r.Rare,
            CostlyInitiate = r.CostlyInitiate,
            Organize = r.Organize,
            CompetencyResult = r.CompetencyResult
        });

    private IQueryable<Resource> ByLevelPlanFilter(long levelId, long planId, string? filter) =>
        Query.Where(r => r.PlaningLevelId == levelId && r.PlanId == planId
                         && (filter == null || r.Title.Contains(filter)));

    public async Task<PagedResult<ResourceDto>> FindAllPageAsync(long levelId, long planId, string? filter, PageRequest pageable, CancellationToken ct = default)
    {
        var query = ProjectBasic(ByLevelPlanFilter(levelId, planId, filter));
        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageable).ToListAsync(ct);
        return new PagedResult<ResourceDto> { Items = items, TotalElements = total, Page = pageable.Page, Size = pageable.Size };
    }

    public async Task<List<ResourceDto>> FindAllAsync(long levelId, long planId, string? filter, CancellationToken ct = default) =>
        await ProjectBasic(ByLevelPlanFilter(levelId, planId, filter)).ToListAsync(ct);

    public async Task<PagedResult<ResourceDto>> FindKeyResourceByPlanIdAsync(long planId, PageRequest pageable, CancellationToken ct = default)
    {
        var query = ProjectFull(Query.Where(r => r.PlaningLevel != null && r.PlaningLevel.Operational == false
                                                 && r.PlanId == planId && r.Power >= 6 && r.Power <= 10));
        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageable).ToListAsync(ct);
        return new PagedResult<ResourceDto> { Items = items, TotalElements = total, Page = pageable.Page, Size = pageable.Size };
    }

    public async Task<List<ResourceDto>> GetKeyResourceByPlanIdListAsync(long planId, CancellationToken ct = default) =>
        await ProjectFull(Query.Where(r => r.PlaningLevel != null && r.PlaningLevel.Operational == false && r.PlanId == planId))
            .ToListAsync(ct);

    public async Task<List<ResourceDto>> GetKeyResourceByPlanIdListIrisaAsync(long planId, CancellationToken ct = default) =>
        await ProjectFull(Query.Where(r => r.Importance >= 6 && r.Importance <= 10 && r.PlanId == planId))
            .ToListAsync(ct);
}
