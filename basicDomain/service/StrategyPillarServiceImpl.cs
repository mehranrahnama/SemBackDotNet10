#nullable enable
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Models.Entities;

namespace SemBackDotNet10.service.impl;

public sealed class StrategyPillarServiceImpl(ServiceRuntime runtime) : ServiceBase<StrategyPillar>(runtime)
{
    private IQueryable<StrategyPillar> ByPlan(long? planId) => GetRepository().Include(x => x.PlaningLevel).Where(x => x.PlanId == planId);
    public async Task<StrategyPillar?> FindStrategyPillarDataByPlanIdAndLevelIdAsync(long? planId, long? levelId, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await FindStrategyPillarDataByPlanIdAndLevelIdForRoadMapAsync(planId, levelId, ct); }
    public Task<StrategyPillar?> FindStrategyPillarDataByPlanIdAndLevelIdForRoadMapAsync(long? planId, long? levelId, CancellationToken ct = default)
        => ByPlan(planId).SingleOrDefaultAsync(x => x.PlaningLevel.Id == levelId, ct);
    public Task<StrategyPillar?> FindStrategyPillarDataByPlanIdAsync(long? planId, CancellationToken ct = default)
        => ByPlan(planId).SingleOrDefaultAsync(ct);
    public Task<StrategyPillarDto> StrategyPillarCreateAsync(StrategyPillarDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto); await AccessAsync(dto.LevelId, ct);
            var entity = (R.TenantName == "MSCHOLDING"
                ? await FindStrategyPillarDataByPlanIdAndLevelIdForRoadMapAsync(dto.PlanId, dto.LevelId, ct)
                : await FindStrategyPillarDataByPlanIdAsync(dto.PlanId, ct)) ?? new StrategyPillar();
            entity.PlanId = dto.PlanId; entity.Philosophy = dto.Philosophy; entity.Mission = dto.Mission; entity.Vision = dto.Vision;
            if (dto.CorporationValues is not null) entity.CorporationValues = dto.CorporationValues;
            if (dto.AbbreviationValue is not null) entity.AbbreviationValue = dto.AbbreviationValue;
            if (dto.BigObjects is not null) entity.BigObjects = dto.BigObjects;
            if (dto.LevelId is not null) entity.PlaningLevel = await RequireAsync<PlaningLevel>(dto.LevelId, "error.PlaningLevel.idIsNotValid", ct);
            await SaveAsync(entity, ct); return MapToDto(entity)!;
        }, ct);
    public StrategyPillarDto? MapToDto(StrategyPillar? entity) => entity is null ? null : new()
    {
        Id = entity.Id, PlanId = entity.PlanId, LevelId = entity.PlaningLevel?.Id,
        Philosophy = entity.Philosophy, Mission = entity.Mission, Vision = entity.Vision,
        CorporationValues = entity.CorporationValues, AbbreviationValue = entity.AbbreviationValue, BigObjects = entity.BigObjects
    };
    public Task<StrategyPillar> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
        => RequireAsync<StrategyPillar>(id, "error.StrategyObject.idIsNotValid", ct);
}
