#nullable enable
using DomainLevel = SemBackDotNet10.basicDomain.Model.Entity.PlaningLevel;
using DomainStrategyDto = SemBackDotNet10.basicDomain.Model.Dto.StrategyObjectDto;
using DomainStrategyObject = SemBackDotNet10.basicDomain.Model.Entity.StrategyObject;
using Common.exception;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.baseDomain.mapper;
using StrategyReadRepository = global::SemBackDotNet10.baseDomain.repository.IStrategyObjectRepository;
using ActionPlan = SemBackDotNet10.actionDomain.model.entity.ActionPlan;

namespace SemBackDotNet10.service.impl;

public sealed class StrategyObjectImpl(ServiceRuntime runtime, IServiceProvider services,
    IStrategyObjectMapper mapper, PerspectiveServiceImpl perspectives,
    StrategyReadRepository strategyRepository) : ServiceBase<DomainStrategyObject>(runtime)
{
    private IRemainingServiceQueries queries => services.GetRequiredService<IRemainingServiceQueries>();
    private IRemainingExternalServices external => services.GetRequiredService<IRemainingExternalServices>();
    private PerspectiveServiceImpl Perspectives => perspectives;
    private IQueryable<DomainStrategyObject> Graph => GetRepository().Include(x => x.PlaningLevel).Include(x => x.PlanPeriod).Include(x => x.Perspective);
    public async Task<DomainStrategyObject> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
        => await Graph.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new BusinessException("error.StrategyObject.idIsNotValid");
    private async Task EditableAsync(long? planId, CancellationToken ct)
    { if (!R.User.IsAdmin && !await R.Db.Set<global::SemBackDotNet10.basicDomain.Model.Entity.PlanPeriod>().AnyAsync(period => period.PlanId == planId && period.StatusType == 2, ct)) throw new BusinessException("دوره برنامه ریزی انتخاب شده فعال نمیباشد."); }
    private async Task ValidateAsync(DomainStrategyDto dto, long? exclude, CancellationToken ct)
    {
        if (dto.Title == null || dto.LevelId == 0 || dto.PlanId == null) throw new BusinessException("مقادیر عنوان، منظر و کمیته برنامه ریزی نمیتوانند خالی باشند.");
        var others = GetRepository().Where(x => x.Id != (exclude ?? 0) && x.PlaningLevel.Id == dto.LevelId && x.PlanPeriod.PlanId == dto.PlanId);
        if (await others.AnyAsync(x => x.Title == dto.Title, ct)) throw new BusinessException("هدف در این سطح و دوره برنامه ریزی تکراری می باشد!");
        if (!string.IsNullOrWhiteSpace(dto.Code) && await others.AnyAsync(x => x.Code == dto.Code, ct)) throw new BusinessException("کد هدف وارد شده تکراری میباشد!");
        if (dto.PerspectId != null && dto.Weight != null)
        {
            var sum = await GetRepository().Where(x => x.Deleted == false && x.Perspective.Id == dto.PerspectId && x.Id != (exclude ?? 0)).SumAsync(x => (double?)x.Weight, ct) ?? 0;
            if (!double.IsFinite((double)dto.Weight) || dto.Weight < 0 || (decimal)sum + (decimal)dto.Weight > 100m) throw new BusinessException("مجموع وزن اهداف این منظر نمی‌تواند بیشتر از 100 باشد.");
        }
    }
    private async Task ApplyAsync(DomainStrategyObject x, DomainStrategyDto dto, CancellationToken ct)
    {
        x.MainStrategyId = dto.MainStrategyId; x.Code = dto.Code; x.OriginId = dto.OriginId;
        x.PlaningLevel = await RequireAsync<DomainLevel>(dto.LevelId, "error.planingLevel.idIsNotValid", ct);
        x.PlanPeriod = await R.External.FindPlanPeriodByPlanIdAsync(dto.PlanId, ct);
        x.Perspective = await Perspectives.FindByIdOrElseThrowAsync(dto.PerspectId, ct);
        if (x.Perspective.PlaningLevel.Id != dto.LevelId || x.Perspective.PlanPeriod.PlanId != dto.PlanId) throw new BusinessException("منظر با کمیته و دوره انتخاب‌شده مطابقت ندارد.");
        x.Title = dto.Title; x.Weight = dto.Weight; x.ThemeId = dto.ThemeId;
    }
    public Task<DomainStrategyObject> StrategyObjectCreateAsync(DomainStrategyDto dto, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        ArgumentNullException.ThrowIfNull(dto); await AccessAsync(dto.LevelId, ct); await EditableAsync(dto.PlanId, ct); await ValidateAsync(dto, null, ct);
        var x = new DomainStrategyObject { Deleted = false }; await ApplyAsync(x, dto, ct); return await SaveAsync(x, ct);
    }, ct);
    public Task<DomainStrategyObject> StrategyObjectEditAsync(DomainStrategyDto dto, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        ArgumentNullException.ThrowIfNull(dto); var x = await FindByIdOrElseThrowAsync(dto.Id, ct);
        await AccessAsync(x.PlaningLevel.Id, ct); await AccessAsync(dto.LevelId, ct);
        await EditableAsync(x.PlanPeriod.PlanId, ct); await EditableAsync(dto.PlanId, ct); await ValidateAsync(dto, x.Id, ct);
        await ApplyAsync(x, dto, ct); return await SaveAsync(x, ct);
    }, ct);
    public async Task<DomainStrategyDto> MapToDtoAsync(DomainStrategyObject entity, CancellationToken ct = default)
    {
        var x = await FindByIdOrElseThrowAsync(entity.Id, ct);
        return new DomainStrategyDto { Id = x.Id, MainStrategyId = x.MainStrategyId, Code = x.Code, LevelId = x.PlaningLevel.Id,
            OriginId = x.OriginId, PlanId = x.PlanPeriod.PlanId, PerspectId = x.Perspective.Id, ThemeId = x.ThemeId, Title = x.Title, Weight = x.Weight };
    }
    public Task<ServicePage<DomainStrategyDto>> FindAllAsync(string? filter, ServicePageRequest page, CancellationToken ct = default) => queries.StrategyObjectsAsync(filter, null, null, null, page, ct);
    public async Task<ServicePage<DomainStrategyDto>> FindAllByLevelIdAsync(long levelId, long planId, string? filter, ServicePageRequest page, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await queries.StrategyObjectsAsync(filter, levelId, planId, null, page, ct); }
    public async Task<ServicePage<DomainStrategyDto>> FindAllByLevelIdandPerspectIdAsync(long levelId, long planId, long perspectiveId, string? filter, ServicePageRequest page, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await queries.StrategyObjectsAsync(filter, levelId, planId, perspectiveId, page, ct); }
    public async Task<List<StrategyObjectPerspectiveDto>> FindAllWithPerspectiveAndThemeAsync(long levelId, long planId, string? filter, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await strategyRepository.FindAllWithPerspectiveAndThemeAsync(levelId, planId, filter, ct); }
    public async Task<List<StrategyObjectTitleDto>> FindAllByLevelIdAndPlanIdAsync(long levelId, long planId, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await strategyRepository.FindAllByLevelIdAndPlanIdAsync(planId, levelId, ct); }
    public Task<long> StrategyObjectCountAsync(long planId, CancellationToken ct = default) => queries.StrategyCountAsync(planId, null, ct);
    public async Task<long> CountActiveGoalsByPlanIdLevelIdAsync(long planId, long levelId, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await queries.StrategyCountAsync(planId, levelId, ct); }
    public async Task<List<DomainStrategyObject>> GetParentStrategyObjAsync(long levelId, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await strategyRepository.FindParentStrategyObjAsync(levelId, ct); }
    public Task<List<DomainStrategyObject>> FindDeletedStrategyObjectByPerspectiveIdAsync(long id, CancellationToken ct = default) => Graph.IgnoreQueryFilters().Where(x => x.Perspective.Id == id && x.Deleted == true).ToListAsync(ct);
    public Task<List<DomainStrategyObject>> FindStrategyObjectByPerspectiveIdAsync(long id, CancellationToken ct = default) => Graph.Where(x => x.Perspective.Id == id && x.Deleted == false).ToListAsync(ct);
    public new Task DeleteByIdAsync(long id, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        var x = await FindByIdOrElseThrowAsync(id, ct); await AccessAsync(x.PlaningLevel.Id, ct);
        var hasDependencies = await R.Db.Database.SqlQuery<int>($"""
            SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.tbldimmeasure WHERE strategyobjectid = {id} AND isdeleted = 0)
                          OR EXISTS (SELECT 1 FROM dbo.tblactionplan WHERE strategyobjectid = {id} AND isdeleted = 0)
                        THEN 1 ELSE 0 END AS Value
            """).SingleAsync(ct);
        if (hasDependencies != 0) throw new BusinessException("امکان حذف هدف به دلیل استفاده در شاخص یا اقدام وجود ندارد.");
        var deleted = await Graph.IgnoreQueryFilters().Where(y => y.Id != id && y.Deleted == true && y.PlaningLevel.Id == x.PlaningLevel.Id && y.PlanPeriod.PlanId == x.PlanPeriod.PlanId && y.Title == x.Title).ToListAsync(ct);
        GetRepository().RemoveRange(deleted); await R.Db.SaveChangesAsync(ct); x.Deleted = true; await SaveAsync(x, ct);
    }, ct);
    public async Task<DomainStrategyObject?> GetStrategyObjectViaDetailsForPlanningLevelsAsync(long planId, string title, DomainLevel level, CancellationToken ct = default)
    {
        var rows = await Graph.Where(x => x.PlaningLevel.Id == level.Id && x.PlanPeriod.PlanId == planId && x.Title == title).Take(2).ToListAsync(ct);
        if (rows.Count > 1) throw new BusinessException("هدف به صورت تکراری در این کمیته و دوره برنامه ریزی تعریف شده است!");
        return rows.SingleOrDefault();
    }
    // Database unique constraint on (tenant, level, period, title) is mandatory.
    // On a unique conflict roll back and retry the WHOLE transaction in a fresh scoped context.
    public Task<DomainStrategyObject> FindOrCreateStrategyObjectAsync(DomainStrategyObject source, DomainLevel level, global::SemBackDotNet10.basicDomain.Model.Entity.PlanPeriod period, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            await AccessAsync(level.Id, ct);
            var existing = await GetStrategyObjectViaDetailsForPlanningLevelsAsync(period.PlanId, source.Title, level, ct);
            if (existing != null) return existing;
            var perspective = await Perspectives.FindOrCreatePerspectiveAsync(source.Perspective, period, level, ct);
            var clone = mapper.CloneForTarget(source, level, period, perspective) ?? throw new InvalidOperationException("Mapper returned null.");
            return await SaveAsync(clone, ct);
        }, ct);
    public Task<DomainStrategyObject> CreateStrategyObjectForLevelIdAsync(DomainStrategyObject source, DomainLevel level, CancellationToken ct = default)
        => FindOrCreateStrategyObjectAsync(source, level, source.PlanPeriod, ct);
    public Task<DomainStrategyObject> FindStrategyObjectWithChangedPlanningLevelAsync(DomainStrategyObject source, DomainLevel level, CancellationToken ct = default)
        => CreateStrategyObjectForLevelIdAsync(source, level, ct);
    public Task<DomainStrategyObject> FindByIdAndPlanIdOrElseThrowAsync(long id, long planId, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        var source = await FindByIdOrElseThrowAsync(id, ct); await AccessAsync(source.PlaningLevel.Id, ct);
        var existing = await GetStrategyObjectViaDetailsForPlanningLevelsAsync(planId, source.Title, source.PlaningLevel, ct);
        if (existing != null) return existing;
        var perspective = await Perspectives.FindByIdAndPlanIdOrElseThrowAsync(source.Perspective.Id, planId, ct);
        var clone = new DomainStrategyObject { MainStrategyId = source.MainStrategyId, Code = source.Code, PlaningLevel = source.PlaningLevel,
            OriginId = source.OriginId, PlanPeriod = await R.External.FindPlanPeriodByPlanIdAsync(planId, ct), Perspective = perspective,
            Title = source.Title, Weight = source.Weight, ThemeId = source.ThemeId, Deleted = false };
        return await SaveAsync(clone, ct);
    }, ct);
    public async Task<List<long>> FindStrategyObjectByTitleAndLevelIdAsync(long id, long levelId, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); var source = await FindByIdOrElseThrowAsync(id, ct); return await GetRepository().Where(x => x.Title == source.Title && x.PlaningLevel.Id == levelId).Select(x => x.Id).ToListAsync(ct); }
    public Task<List<DomainStrategyObject>> InsertStrategyObjectAndActionPlanIfNotExistAsync(long planId, long lastPlanId, ActionPlan actionPlan, CancellationToken ct = default) => R.TransactionAsync(async () =>
    {
        var result = new List<DomainStrategyObject>(); await AccessAsync(actionPlan.PlaningLevel.Id, ct);
        foreach (var ap in await queries.ActionPlansByCatalogAndPeriodAsync(actionPlan, ct))
        {
            var target = await GetStrategyObjectViaDetailsForPlanningLevelsAsync(planId, ap.StrategyObject.Title, actionPlan.PlaningLevel, ct);
            if (target == null)
            {
                var previous = await GetStrategyObjectViaDetailsForPlanningLevelsAsync(lastPlanId, ap.StrategyObject.Title, actionPlan.PlaningLevel, ct)
                    ?? throw new BusinessException("هدف دوره قبل یافت نشد.");
                target = await FindByIdAndPlanIdOrElseThrowAsync(previous.Id, planId, ct);
            }
            result.Add(target); await external.CopyActionPlanAsync(target, planId, ap, ct);
        }
        return result;
    }, ct);
}
