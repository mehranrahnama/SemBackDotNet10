using global::SemBackDotNet10.basicDomain.Model.Entity;
using DomainPeriod = global::SemBackDotNet10.basicDomain.Model.Entity.PlanPeriod;
#nullable enable
using Common.exception;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.baseDomain.mapper;


namespace SemBackDotNet10.service.impl;

/// <summary>Perspective/theme operations. Requires a tenant-filtered, shared scoped DbContext.</summary>
public sealed class PerspectiveServiceImpl(ServiceRuntime runtime, IPerspectiveMapper mapper)
    : ServiceBase<Perspective>(runtime)
{
    private IQueryable<Perspective> Loaded => GetRepository().Include(x => x.Parent)
        .Include(x => x.PlaningLevel).Include(x => x.PlanPeriod);

    public Task<Perspective> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
        => FindRequiredAsync(id, ct);

    private async Task<Perspective> FindRequiredAsync(long? id, CancellationToken ct)
        => id is null ? throw new BusinessException("error.perspective.find")
            : await Loaded.SingleOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new BusinessException("error.perspective.find");

    private Task<Perspective?> MatchAsync(Perspective sample, long? planId, long? levelId, CancellationToken ct)
        => Loaded.SingleOrDefaultAsync(x => x.DesperspectF == sample.DesperspectF
            && x.PlanPeriod.PlanId == planId && x.PlaningLevel.Id == levelId, ct);

    private Task<int> MainCountAsync(long? levelId, long? planId, CancellationToken ct)
        => GetRepository().CountAsync(x => x.Parent == null && x.PlaningLevel.Id == levelId
            && x.PlanPeriod.PlanId == planId, ct);

    // DbUpdateException is not necessarily a duplicate-key violation. Do not mislabel every
    // provider error as a duplicate; configure provider-specific exception translation at the boundary.
    private Task<Perspective> CreateAsync(Perspective entity, CancellationToken ct) => SaveAsync(entity, ct);

    public Task<Perspective> SaveAsync(PerspectiveDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            await AccessAsync(dto.Levelid, ct);
            var level = await RequireAsync<PlaningLevel>(dto.Levelid, "error.PlaningLevel.idIsNotValid", ct);
            await R.Db.Entry(level).Reference(x => x.Parent).LoadAsync(ct);
            Perspective? parent = null;
            if (dto.ParentId is null)
            {
                if (R.TenantName == "MSCHOLDING" && level.Parent is not null)
                    throw new BusinessException("تعریف منظر فقط در سطح برنامه ریزی گروه فولاد مبارکه امکانپذیر است. ");
                if (R.TenantName == "MSCHOLDING" && !R.User.IsAdmin)
                    throw new BusinessException("تعریف منظر فقط برای کاربر ادمین امکانپذیر است. ");
                if (await MainCountAsync(dto.Levelid, dto.Planid, ct) >= 4)
                    throw new BusinessException("امکان تعریف بیشتر از 4 منظر اصلی برای هر کمیته وجود ندارد");
            }
            else parent = await FindRequiredAsync(dto.ParentId, ct);
            var period = await R.External.FindPlanPeriodByPlanIdAsync(dto.Planid, ct);
            var result = await CreateAsync(new Perspective {
                DesperspectF = dto.DesperspectF, Parent = parent,
                Orders = dto.ParentId is null ? dto.Orders : null, PlaningLevel = level, PlanPeriod = period
            }, ct);
            if (dto.ParentId is null && R.TenantName == "MSCHOLDING")
            {
                var children = await R.Db.Set<PlaningLevel>().Where(x => x.Parent != null).ToListAsync(ct);
                foreach (var child in children)
                    await CreateAsync(new Perspective { DesperspectF = dto.DesperspectF,
                        Orders = dto.Orders, PlaningLevel = child, PlanPeriod = period }, ct);
            }
            return result;
        }, ct);

    public PerspectiveDto MapToDto(Perspective entity) => new() {
        DesperspectF = entity.DesperspectF, Orders = entity.Orders,
        Levelid = entity.PlaningLevel.Id, DesLevel = entity.PlaningLevel.DesLevel,
        Planid = entity.PlanPeriod.PlanId, ParentId = entity.Parent?.Id
    };

    public Task<Perspective> EditAsync(PerspectiveDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            await AccessAsync(dto.Levelid, ct);
            var entity = await FindRequiredAsync(dto.Id, ct);
            // Also authorize the stored level; a caller must not bypass access using a different DTO level.
            await AccessAsync(entity.PlaningLevel.Id, ct);
            if (dto.ParentId is null && await MainCountAsync(dto.Levelid, dto.Planid, ct) > 4)
                throw new BusinessException("امکان تعریف بیشتر از 4 منظر اصلی برای هر کمیته وجود ندارد");
            var parent = dto.ParentId is null ? null : await FindRequiredAsync(dto.ParentId, ct);
            if (parent is not null)
                foreach (var ancestor in await PathAsync(parent, ct))
                    if (ancestor.Id == entity.Id) throw new BusinessException("چرخه در سلسله مراتب منظر مجاز نیست.");
            entity.DesperspectF = dto.DesperspectF;
            entity.Parent = parent;
            entity.Orders = parent is null ? dto.Orders : null;
            return await SaveAsync(entity, ct);
        }, ct);

    public Task DeletePerspectiveAsync(long id, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            var entity = await FindRequiredAsync(id, ct);
            await AccessAsync(entity.PlaningLevel.Id, ct);
            if (await R.External.HasActiveStrategiesByPerspectiveAsync(id, ct))
                throw new BusinessException("مضمون/منظر مورد نظر دارای هدف فعال است.");
            if (entity.Parent is null) throw new BusinessException("error.perspective.delete.ismain");
            var deleted = await R.External.DeletedStrategiesByPerspectiveAsync(id, ct);
            await R.External.HardDeleteActionPlansAsync(deleted, ct);
            await R.External.HardDeleteMeasuresAsync(deleted, ct);
            R.Db.Set<StrategyObject>().RemoveRange(deleted);
            GetRepository().Remove(entity);
            await R.Db.SaveChangesAsync(ct);
        }, ct);

    public Task EditAllWeightsAsync(IReadOnlyList<PerspectiveDto> dtos, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            foreach (var dto in dtos)
            {
                var entity = await FindRequiredAsync(dto.Id, ct);
                await AccessAsync(entity.PlaningLevel.Id, ct);
                entity.Weight = dto.Weight;
            }
            await R.Db.SaveChangesAsync(ct);
        }, ct);

    public async Task<ServicePage<PerspectiveDto>> FindAllAsync(long levelId, long planId,
        string? filter, ServicePageRequest page, CancellationToken ct = default)
    {
        _ = page.Offset;
        await AccessAsync(levelId, ct);
        return await R.Queries.PerspectivesAsync(levelId, planId, filter, page, ct);
    }

    // Child -> parent, explicitly loading every ancestor rather than assuming lazy loading.
    private async Task<List<Perspective>> PathAsync(Perspective source, CancellationToken ct)
    {
        var path = new List<Perspective>();
        var visited = new HashSet<Perspective>(ReferenceEqualityComparer.Instance);
        var ids = new HashSet<long>();
        Perspective? current = source;
        while (current is not null)
        {
            ct.ThrowIfCancellationRequested();
            if (!visited.Add(current) || (current.Id != 0 && !ids.Add(current.Id)))
                throw new BusinessException("چرخه در سلسله مراتب منظر مجاز نیست.");
            path.Add(current);
            if (R.Db.Entry(current).State != EntityState.Detached)
            {
                await R.Db.Entry(current).Reference(x => x.Parent).LoadAsync(ct);
                await R.Db.Entry(current).Reference(x => x.PlaningLevel).LoadAsync(ct);
                await R.Db.Entry(current).Reference(x => x.PlanPeriod).LoadAsync(ct);
            }
            current = current.Parent;
        }
        return path;
    }

    public Task<Perspective> FindByIdAndPlanIdOrElseThrowAsync(long? id, long? planId, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            var source = await FindRequiredAsync(id, ct);
            if (source.PlanPeriod.PlanId == planId) return source;
            var existing = await MatchAsync(source, planId, source.PlaningLevel.Id, ct);
            if (existing is not null && ((existing.Parent is null && source.Parent is null)
                || (existing.Parent is not null && source.Parent is not null
                    && existing.Parent.DesperspectF == source.Parent.DesperspectF))) return existing;
            return await CreateWithParentAsync(source, planId, ct);
        }, ct);

    public Task<Perspective> CreateWithParentAsync(Perspective source, long? planId, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            var period = await R.External.FindPlanPeriodByPlanIdAsync(planId, ct);
            Perspective? parent = null;
            if (source.Parent is not null)
            {
                var sample = await FindRequiredAsync(source.Parent.Id, ct);
                parent = await MatchAsync(sample, planId, sample.PlaningLevel.Id, ct);
                if (parent is null) parent = await CreateAsync(Copy(sample, period, sample.PlaningLevel, null), ct);
            }
            return await CreateAsync(Copy(source, period, source.PlaningLevel, parent), ct);
        }, ct);

    private static Perspective Copy(Perspective source, DomainPeriod period, PlaningLevel level, Perspective? parent)
        => new() { DesperspectF = source.DesperspectF, DesperspectE = source.DesperspectE,
            Code = source.Code, OriginId = source.OriginId, Orders = source.Orders,
            Rank = source.Rank, Weight = source.Weight, PlanPeriod = period, PlaningLevel = level, Parent = parent };

    public Task<Perspective> CreatePerspectiveIfNotExistsForPlanningLevelAsync(Perspective source,
        PlaningLevel level, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            var existing = await MatchAsync(source, source.PlanPeriod.PlanId, level.Id, ct);
            if (existing is not null) return existing;
            var path = await PathAsync(source, ct);
            path.Reverse();
            Perspective? last = null;
            foreach (var node in path)
            {
                var match = await MatchAsync(node, node.PlanPeriod.PlanId, level.Id, ct);
                last = match ?? await CreateAsync(mapper.CloneForPlanningLevel(node, level, last)
                    ?? throw new InvalidOperationException("Perspective mapper returned null."), ct);
            }
            return last ?? throw new BusinessException("error.perspective.find");
        }, ct);

    public Task<Perspective> CopyPerspectiveIfNotExistForPlanPeriodAsync(DomainPeriod period,
        StrategyObject strategy, CancellationToken ct = default)
    {
        var source = mapper.CloneForPlanningLevel(strategy.Perspective, strategy.PlaningLevel, strategy.Perspective.Parent)
            ?? throw new InvalidOperationException("Perspective mapper returned null.");
        source.PlanPeriod = period;
        source.PlanId = period.PlanId;
        return CreatePerspectiveIfNotExistsForPlanningLevelAsync(source, source.PlaningLevel, ct);
    }

    public Task<Perspective> FindOrCreatePerspectiveAsync(Perspective source, DomainPeriod period,
        PlaningLevel level, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            var existing = await MatchAsync(source, period.PlanId, level.Id, ct);
            if (existing is not null) return existing;
            var path = await PathAsync(source, ct);
            path.Reverse();
            Perspective? parent = null;
            foreach (var node in path)
                parent = await MatchAsync(node, period.PlanId, level.Id, ct)
                    ?? await CreateAsync(Copy(node, period, level, parent), ct);
            return parent ?? throw new BusinessException("error.perspective.find");
        }, ct);

    // Typed report arguments replace JSON parsing at the service boundary.
    public async Task<List<StrategyMap>> StrategyMapReportServiceAsync(long? planId, long? levelId,
        long? month, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await R.Queries.StrategyMapReportAsync(planId, levelId, month, ct); }

    public async Task<List<StrategyMapDashboard>> StrategyMapDashboardReportServiceAsync(long? planId,
        long? levelId, long? month, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await R.Queries.StrategyMapDashboardReportAsync(planId, levelId, month, ct); }

    public async Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasureDataByStrategyObjectIdControllerAsync(
        long? objectId, long? planId, long? levelId, long? month, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await R.Queries.ReportMeasuresAsync(objectId, planId, levelId, month, false, ct); }

    public async Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasureDataByStrategyObjectIdAllMonthControllerAsync(
        long? objectId, long? planId, long? levelId, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await R.Queries.ReportMeasuresAsync(objectId, planId, levelId, null, true, ct); }

    public async Task<List<ReportActionDataByStrategyObjectId>> ReportActionDataByStrategyObjectIdControllerAsync(
        long? objectId, long? planId, long? levelId, long? month, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await R.Queries.ReportActionsAsync(objectId, planId, levelId, month, false, ct); }

    public async Task<List<ReportActionDataByStrategyObjectId>> ReportActionDataByStrategyObjectIdAllMonthControllerAsync(
        long? objectId, long? planId, long? levelId, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await R.Queries.ReportActionsAsync(objectId, planId, levelId, null, true, ct); }
}
