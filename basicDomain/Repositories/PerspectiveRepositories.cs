using global::SemBackDotNet10.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Entity;
using global::Common.model.dto;
using static global::Common.utils.PageUtils;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.basicDomain.Repositories;

// ---------- Perspective ----------

public interface IPerspectiveRepository : IBaseRepository<Perspective>
{
    Task<long> FindCountOfMainPerspectAsync(long levelId, long planId, CancellationToken ct = default);

    Task<Perspective?> FindByDesperspectFAndPlanIdAndPlaningLevelIdAsync(string desperspectF, long planId, long levelId, CancellationToken ct = default);

    Task<PagedResult<PerspectiveDto>> FindByLevelIdAndPlanIdAsync(long levelId, long planId, string? filter, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default);
}

public class PerspectiveRepository : BaseRepository<Perspective>, IPerspectiveRepository
{
    public PerspectiveRepository(SemDbContext context) : base(context) { }

    public Task<long> FindCountOfMainPerspectAsync(long levelId, long planId, CancellationToken ct = default) =>
        Context.Perspectives
            .Where(p => p.PlaningLevelId == levelId && p.PlanPeriodId != null && p.PlanPeriodId.Value == checked((int)planId) && p.ParentId == null)
            .LongCountAsync(ct);

    public Task<Perspective?> FindByDesperspectFAndPlanIdAndPlaningLevelIdAsync(string desperspectF, long planId, long levelId, CancellationToken ct = default) =>
        Context.Perspectives.FirstOrDefaultAsync(
            p => p.DesperspectF == desperspectF && p.PlanPeriod.PlanId == planId && p.PlaningLevelId == levelId, ct);

    public async Task<PagedResult<PerspectiveDto>> FindByLevelIdAndPlanIdAsync(long levelId, long planId, string? filter, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Context.Perspectives
            .Where(p => p.PlaningLevelId == levelId && p.PlanPeriod != null && p.PlanPeriod.PlanId == planId)
            .Where(p => filter == null || p.DesperspectF!.Contains(filter))
            .Select(p => new PerspectiveDto
            {
                Id = p.Id,
                DesperspectF = p.DesperspectF,
                Orders = p.Orders,
                Levelid = p.PlaningLevelId,
                DesLevel = p.PlaningLevel!.DesLevel,
                Planid = p.PlanPeriod!.PlanId,
                ParentId = p.ParentId
            });

        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageRequest).ToListAsync(ct);
        return new PagedResult<PerspectiveDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }
}

// ---------- MainStrategy ----------

public interface IMainStrategyRepository : IBaseRepository<MainStrategy>
{
    Task<PagedResult<MainStrategyDto>> FindAllMainStrategiesAsync(string? filter, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default);

    Task<PagedResult<MainStrategyDto>> FindAllMainStrategiesByLevelIdAsync(string? filter, long? levelId, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default);

    Task<long> CountActiveStrategyAsync(long planId, CancellationToken ct = default);

    Task<long> CountActiveStrategyByLevelIdAsync(long levelId, CancellationToken ct = default);
}

public class MainStrategyRepository : BaseRepository<MainStrategy>, IMainStrategyRepository
{
    public MainStrategyRepository(SemDbContext context) : base(context) { }

    public async Task<PagedResult<MainStrategyDto>> FindAllMainStrategiesAsync(string? filter, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = from s in Context.MainStrategies
                    where filter == null || s.DesStrategyF!.Contains(filter)
                    select new MainStrategyDto
                    {
                        Id = s.Id,
                        DesStrategyF = s.DesStrategyF,
                        DateStart = s.DateStart,
                        DateEnd = s.DateEnd,
                        PerspectiveId = s.PerspectiveId,
                        PerspectiveDescription = s.Perspective!.DesperspectF,
                        LevelId = s.PlaningLevelId,
                        AccountableId = s.AccountableId,
                        AccountableName = s.Accountable == null
                            ? null
                            : s.Accountable.NameF + " " + s.Accountable.FamilyF
                    };

        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageRequest).ToListAsync(ct);
        return new PagedResult<MainStrategyDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public async Task<PagedResult<MainStrategyDto>> FindAllMainStrategiesByLevelIdAsync(string? filter, long? levelId, global::Common.utils.PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = from s in Context.MainStrategies
                    where s.PlaningLevelId == levelId
                          && (filter == null || s.DesStrategyF!.Contains(filter))
                    select new MainStrategyDto
                    {
                        Id = s.Id,
                        DesStrategyF = s.DesStrategyF,
                        DateStart = s.DateStart,
                        DateEnd = s.DateEnd,
                        PerspectiveId = s.PerspectiveId,
                        PerspectiveDescription = s.Perspective!.DesperspectF,
                        LevelId = s.PlaningLevelId,
                        AccountableId = s.AccountableId,
                        AccountableName = s.Accountable == null
                            ? null
                            : s.Accountable.NameF + " " + s.Accountable.FamilyF
                    };

        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageRequest).ToListAsync(ct);
        return new PagedResult<MainStrategyDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public async Task<long> CountActiveStrategyAsync(long planId, CancellationToken ct = default)
    {
        var values = await Context.Database.SqlQuery<long?>($"EXEC [dbo].[CountActiveStrategy]").ToListAsync(ct);
        return values.Single() ?? throw new InvalidOperationException("CountActiveStrategy returned NULL.");
    }

    public async Task<long> CountActiveStrategyByLevelIdAsync(long levelId, CancellationToken ct = default)
    {
        var values = await Context.Database.SqlQuery<long?>($"EXEC [dbo].[CountActiveStrategyByLevelId] {levelId}").ToListAsync(ct);
        return values.Single() ?? throw new InvalidOperationException("CountActiveStrategyByLevelId returned NULL.");
    }
}

// ---------- گزارش‌ها (StrategyMap و ...) ----------

public interface IStrategyMapRepository
{
    Task<List<StrategyMap>> StrategyMapReportAsync(long? planId, long? levelId, long? month, CancellationToken ct = default);
}

public class StrategyMapRepository : IStrategyMapRepository
{
    private readonly SemDbContext _context;

    public StrategyMapRepository(SemDbContext context) => _context = context;

    public Task<List<StrategyMap>> StrategyMapReportAsync(long? planId, long? levelId, long? month, CancellationToken ct = default)
    {
        return _context.Database.SqlQuery<StrategyMap>($"EXEC [dbo].[Report_StrategyMap] {planId}, {levelId}, {month}").ToListAsync(ct);
    }
}

public interface IStrategyMapDashboardRepository
{
    Task<List<StrategyMapDashboard>> StrategyMapDashboardReportAsync(long? planId, long? levelId, long? month, CancellationToken ct = default);
}

public class StrategyMapDashboardRepository : IStrategyMapDashboardRepository
{
    private readonly SemDbContext _context;

    public StrategyMapDashboardRepository(SemDbContext context) => _context = context;

    public Task<List<StrategyMapDashboard>> StrategyMapDashboardReportAsync(long? planId, long? levelId, long? month, CancellationToken ct = default)
    {
        return _context.Database.SqlQuery<StrategyMapDashboard>($"EXEC [dbo].[Report_StrategyMapDashboard] {planId}, {levelId}, {month}").ToListAsync(ct);
    }
}

public interface IReportMeasureDataByStrategyObjectIdRepository
{
    Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasureDataByStrategyObjectIdAsync(long? strategyObjectId, long? planId, long? levelId, long? month, CancellationToken ct = default);

    Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasureDataByStrategyObjectIdAllMonthAsync(long? strategyObjectId, long? planId, long? levelId, CancellationToken ct = default);
}

public class ReportMeasureDataByStrategyObjectIdRepository : IReportMeasureDataByStrategyObjectIdRepository
{
    private readonly SemDbContext _context;

    public ReportMeasureDataByStrategyObjectIdRepository(SemDbContext context) => _context = context;

    public Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasureDataByStrategyObjectIdAsync(long? strategyObjectId, long? planId, long? levelId, long? month, CancellationToken ct = default)
    {
        return _context.Database.SqlQuery<ReportMeasureDataByStrategyObjectId>($"EXEC [dbo].[Report_Measure_Hosco_WithStrategyObjectId] {strategyObjectId}, {planId}, {levelId}, {month}").ToListAsync(ct);
    }

    public Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasureDataByStrategyObjectIdAllMonthAsync(long? strategyObjectId, long? planId, long? levelId, CancellationToken ct = default)
    {
        return _context.Database.SqlQuery<ReportMeasureDataByStrategyObjectId>($"EXEC [dbo].[Report_Measure_Hosco_WithStrategyObjectIdAllMonth] {strategyObjectId}, {planId}, {levelId}").ToListAsync(ct);
    }
}

public interface IReportActionDataByStrategyObjectIdRepository
{
    Task<List<ReportActionDataByStrategyObjectId>> ReportActionDataByStrategyObjectIdAsync(long? strategyObjectId, long? planId, long? levelId, long? month, CancellationToken ct = default);

    Task<List<ReportActionDataByStrategyObjectId>> ReportActionDataByStrategyObjectIdAllMonthAsync(long? strategyObjectId, long? planId, long? levelId, CancellationToken ct = default);
}

public class ReportActionDataByStrategyObjectIdRepository : IReportActionDataByStrategyObjectIdRepository
{
    private readonly SemDbContext _context;

    public ReportActionDataByStrategyObjectIdRepository(SemDbContext context) => _context = context;

    public Task<List<ReportActionDataByStrategyObjectId>> ReportActionDataByStrategyObjectIdAsync(long? strategyObjectId, long? planId, long? levelId, long? month, CancellationToken ct = default)
    {
        return _context.Database.SqlQuery<ReportActionDataByStrategyObjectId>($"EXEC [dbo].[Report_Action_Hosco_WithStrategyObjectId] {strategyObjectId}, {planId}, {levelId}, {month}").ToListAsync(ct);
    }

    public Task<List<ReportActionDataByStrategyObjectId>> ReportActionDataByStrategyObjectIdAllMonthAsync(long? strategyObjectId, long? planId, long? levelId, CancellationToken ct = default)
    {
        return _context.Database.SqlQuery<ReportActionDataByStrategyObjectId>($"EXEC [dbo].[Report_Action_Hosco_WithStrategyObjectIdAllMonth] {strategyObjectId}, {planId}, {levelId}").ToListAsync(ct);
    }
}
