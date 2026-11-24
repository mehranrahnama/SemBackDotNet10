using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Dto;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.service.impl;

public sealed class StrategyObjectReadService(ServiceRuntime runtime) : ServiceBase<StrategyObject>(runtime)
{
    public async Task<ServicePage<StrategyObjectDto>> FindAsync(long levelId, long planId, long? perspectiveId,
        string? filter, ServicePageRequest page, CancellationToken ct)
    {
        await AccessAsync(levelId, ct);
        var query = GetRepository().AsNoTracking().Where(item => item.Deleted == false &&
            item.PlaningLevelId == levelId && item.PlanPeriod != null && item.PlanPeriod.PlanId == planId &&
            (perspectiveId == null || item.PerspectiveId == perspectiveId) &&
            (filter == null || item.Code!.Contains(filter) || item.Title!.Contains(filter)))
            .Select(item => new StrategyObjectDto
            {
                Id = item.Id, LevelId = item.PlaningLevelId, MainStrategyId = item.MainStrategyId,
                Code = item.Code, Title = item.Title, Weight = item.Weight, ThemeId = item.ThemeId,
                PlanId = item.PlanId, OriginId = item.OriginId, PerspectId = item.PerspectiveId,
                PerspectDes = item.Perspective == null ? null : item.Perspective.DesperspectF
            });
        return await PageAsync(query, page, ct);
    }
}
