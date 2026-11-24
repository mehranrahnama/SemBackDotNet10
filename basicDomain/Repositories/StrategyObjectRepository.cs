#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.webserviceDomain.domain.dto;
using SemBackDotNet10.basicDomain.Model.Dto;


namespace SemBackDotNet10.baseDomain.repository;

public interface IStrategyObjectRepository : ISemRepository<StrategyObject, long>
{
    Task<List<StrategyObject>> FindDeletedStrategyObjectByPerspectiveIdAsync(long perspectiveId, CancellationToken ct = default);
    Task<List<StrategyObject>> FindStrategyObjectByPerspectiveIdAsync(long perspectiveId, CancellationToken ct = default);
    Task<StrategyObject?> FindStrategyObjectByLevelPlanTitleAsync(long planId, long levelId, string title, CancellationToken ct = default);
    Task<PageResult<StrategyObjectDto>> LoadGridAsync(PageRequest page, string? filter, long levelId, long planId, CancellationToken ct = default);
    Task<PageResult<StrategyObjectWebServiceDto>> LoadGridByPlanIdAsync(PageRequest page, long planId, CancellationToken ct = default);
    Task<PageResult<StrategyObjectDto>> LoadGridByLevelIdAndPerspectIdAsync(PageRequest page, string? filter, long levelId, long planId, long perspectId, CancellationToken ct = default);
    Task<long?> GetStrategyObjectCountAsync(long planId, CancellationToken ct = default);
    Task<long?> CountActiveGoalsByPlanIdLevelIdAsync(long planId, long levelId, CancellationToken ct = default);
    Task<List<StrategyObject>> FindParentStrategyObjAsync(long levelId, CancellationToken ct = default);
    Task<List<StrategyObjectPerspectiveDto>> FindAllWithPerspectiveAndThemeAsync(long levelId, long planId, string? filter, CancellationToken ct = default);
    Task<List<StrategyObjectTitleDto>> FindAllByLevelIdAndPlanIdAsync(long planId, long levelId, CancellationToken ct = default);
    Task<List<StrategyObject>> FindAllStrategyObjectByLevelIdAsync(long levelId, long planId, CancellationToken ct = default);
    Task<List<StrategyObject>> FindByPlaningLevelIdAndTitleAndPlanIdAndPerspectiveIdAsync(long levelId, string title, long planId, long perspectId, CancellationToken ct = default);
    Task<StrategyObject?> FindByPlaningLevelIdAndTitleAndPlanIdAndDeletedIsTrueAsync(long levelId, string title, long planId, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, long? id, CancellationToken ct = default);
    Task<StrategyObject?> FindByIdAndActivePlanPeriodAsync(long id, IReadOnlySet<string> userRoles, string adminRole, int activeStatus, CancellationToken ct = default);
    Task<List<StrategyObject>> FindByPlanningLevelIdAndPlanIdAndTitleAsync(long levelId, long planId, string title, CancellationToken ct = default);
    Task<string?> FindTitleByIdAsync(long strategyObjectId, CancellationToken ct = default);
    Task<List<long>> FindByTitleAndLevelIdAsync(string title, long levelId, CancellationToken ct = default);
    Task<bool> ExistsByTitleAndLevelIdAndPlanIdAsync(string title, long levelId, long planId, long? strategyObjectId, CancellationToken ct = default);
    Task<double> SumWeightByPerspectiveIdAsync(long perspectiveId, long? excludeStrategyObjectId, CancellationToken ct = default);
}

public sealed class StrategyObjectRepository(SemDbContext db)
    : SemRepository<StrategyObject, long>(db), IStrategyObjectRepository
{
    public Task<List<StrategyObject>> FindDeletedStrategyObjectByPerspectiveIdAsync(long perspectiveId, CancellationToken ct = default)
    {
        return Set.IgnoreQueryFilters().Where(x => x.PerspectiveId == perspectiveId && x.Deleted == true).ToListAsync(ct);
    }

    public Task<List<StrategyObject>> FindStrategyObjectByPerspectiveIdAsync(long perspectiveId, CancellationToken ct = default)
    {
        return Set.Where(x => x.PerspectiveId == perspectiveId && x.Deleted == false).ToListAsync(ct);
    }

    public Task<StrategyObject?> FindStrategyObjectByLevelPlanTitleAsync(long planId, long levelId, string title, CancellationToken ct = default)
    {
        return Set.Include(x => x.PlanPeriod).SingleOrDefaultAsync(x => x.PlanId == planId && x.PlaningLevelId == levelId && x.Title == title && x.Deleted == false, ct);
    }

    public Task<PageResult<StrategyObjectDto>> LoadGridAsync(PageRequest page, string? filter, long levelId, long planId, CancellationToken ct = default)
    {
        return PageAsync(Set.Where(x => x.Deleted == false && x.PlaningLevelId == levelId && x.PlanId == planId && (filter == null || EF.Functions.Like(x.Code, "%" + filter + "%") || EF.Functions.Like(x.Title, "%" + filter + "%"))).OrderBy(x => x.Id).Select(x => new StrategyObjectDto(x.Id, x.PlaningLevelId, x.MainStrategyId, x.Code, x.Title, x.Weight, x.ThemeId, x.PlanId, x.OriginId, x.PerspectiveId, x.Perspective.DesperspectF)), page, ct);
    }

    public Task<PageResult<StrategyObjectWebServiceDto>> LoadGridByPlanIdAsync(PageRequest page, long planId, CancellationToken ct = default)
    {
        return PageAsync(Set.Where(x => x.Deleted == false && x.PlanId == planId).OrderBy(x => x.Id).Select(x => new StrategyObjectWebServiceDto(x.Id, x.Title)), page, ct);
    }

    public Task<PageResult<StrategyObjectDto>> LoadGridByLevelIdAndPerspectIdAsync(PageRequest page, string? filter, long levelId, long planId, long perspectId, CancellationToken ct = default)
    {
        return PageAsync(Set.Where(x => x.Deleted == false && x.PlaningLevelId == levelId && x.PlanId == planId && (filter == null || EF.Functions.Like(x.Code, "%" + filter + "%") || EF.Functions.Like(x.Title, "%" + filter + "%")) && x.PerspectiveId == perspectId).OrderBy(x => x.Id).Select(x => new StrategyObjectDto(x.Id, x.PlaningLevelId, x.MainStrategyId, x.Code, x.Title, x.Weight, x.ThemeId, x.PlanId, x.OriginId, x.PerspectiveId, x.Perspective.DesperspectF)), page, ct);
    }

    public async Task<long?> GetStrategyObjectCountAsync(long planId, CancellationToken ct = default)
    {
        var values = await Db.Database.SqlQuery<long?>($"EXEC [dbo].[CountActiveGoals] {planId}").ToListAsync(ct);
        return values.SingleOrDefault();
    }

    public async Task<long?> CountActiveGoalsByPlanIdLevelIdAsync(long planId, long levelId, CancellationToken ct = default)
    {
        var values = await Db.Database.SqlQuery<long?>($"EXEC [dbo].[CountActiveGoalsByPlanIdLevelId] {planId}, {levelId}").ToListAsync(ct);
        return values.SingleOrDefault();
    }

    public Task<List<StrategyObject>> FindParentStrategyObjAsync(long levelId, CancellationToken ct = default)
    {
        return Set.IgnoreQueryFilters().Where(x => Db.Set<PlaningLevel>().Where(l => l.Id == levelId && l.ParentId != null).Select(l => l.ParentId).Contains(x.PlaningLevelId)).ToListAsync(ct);
    }

    public Task<List<StrategyObjectPerspectiveDto>> FindAllWithPerspectiveAndThemeAsync(long levelId, long planId, string? filter, CancellationToken ct = default)
    {
        return Set.Where(x => x.Deleted == false && x.PlaningLevelId == levelId && x.PlanId == planId &&
                (filter == null || EF.Functions.Like(x.Code!, "%" + filter + "%") || EF.Functions.Like(x.Title!, "%" + filter + "%")))
            .OrderBy(x => x.Code)
            .Select(x => new StrategyObjectPerspectiveDto(x.Id, x.Code, x.Title, x.Weight,
                x.Perspective!.Parent == null ? x.PerspectiveId : x.Perspective.Parent.Id,
                x.Perspective.Parent == null ? x.Perspective.DesperspectF : x.Perspective.Parent.DesperspectF,
                x.Perspective.Parent == null ? null : (long?)x.Perspective.Id,
                x.Perspective.Parent == null ? null : x.Perspective.DesperspectF))
            .ToListAsync(ct);
    }

    public Task<List<StrategyObjectTitleDto>> FindAllByLevelIdAndPlanIdAsync(long planId, long levelId, CancellationToken ct = default)
    {
        return Set.IgnoreQueryFilters().Where(x => x.PlaningLevelId == levelId && x.PlanId == planId).Select(x => new StrategyObjectTitleDto(x.Id, x.Title)).ToListAsync(ct);
    }

    public Task<List<StrategyObject>> FindAllStrategyObjectByLevelIdAsync(long levelId, long planId, CancellationToken ct = default)
    {
        return Set.Include(x => x.Perspective).Include(x => x.PlaningLevel).Include(x => x.PlanPeriod).Where(x => x.Perspective != null && x.PlaningLevel != null && x.PlanPeriod != null && x.PlaningLevelId == levelId && x.PlanId == planId).ToListAsync(ct);
    }

    public Task<List<StrategyObject>> FindByPlaningLevelIdAndTitleAndPlanIdAndPerspectiveIdAsync(long levelId, string title, long planId, long perspectId, CancellationToken ct = default)
    {
        return Set.Where(x => x.PlaningLevelId == levelId && x.Title == title && x.PlanId == planId && x.PerspectiveId == perspectId && x.Deleted == false).ToListAsync(ct);
    }

    public Task<StrategyObject?> FindByPlaningLevelIdAndTitleAndPlanIdAndDeletedIsTrueAsync(long levelId, string title, long planId, CancellationToken ct = default)
    {
        return Set.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.PlaningLevelId == levelId && x.Title == title && x.PlanId == planId && x.Deleted == true, ct);
    }

    public Task<bool> ExistsByCodeAsync(string code, long? id, CancellationToken ct = default)
    {
        return Set.AnyAsync(x => x.Code == code && (id == null || x.Id != id) && x.Deleted == false, ct);
    }

    public Task<StrategyObject?> FindByIdAndActivePlanPeriodAsync(long id, IReadOnlySet<string> userRoles, string adminRole, int activeStatus, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.Id == id && (x.PlanPeriod.StatusType == activeStatus || userRoles.Contains(adminRole)) && x.Deleted == false, ct);
    }

    public Task<List<StrategyObject>> FindByPlanningLevelIdAndPlanIdAndTitleAsync(long levelId, long planId, string title, CancellationToken ct = default)
    {
        return Set.Include(x => x.PlanPeriod).Where(x => x.Deleted == false && x.PlaningLevelId == levelId && x.PlanId == planId && x.Title == title).ToListAsync(ct);
    }

    public Task<string?> FindTitleByIdAsync(long strategyObjectId, CancellationToken ct = default)
    {
        return Set.Where(x => x.Deleted == false && x.Id == strategyObjectId).Select(x => x.Title).SingleOrDefaultAsync(ct);
    }

    public Task<List<long>> FindByTitleAndLevelIdAsync(string title, long levelId, CancellationToken ct = default)
    {
        return Set.Where(x => x.Title == title && x.PlaningLevelId == levelId && x.Deleted == false).Select(x => x.Id).ToListAsync(ct);
    }

    public Task<bool> ExistsByTitleAndLevelIdAndPlanIdAsync(string title, long levelId, long planId, long? strategyObjectId, CancellationToken ct = default)
    {
        return Set.AnyAsync(x => x.Title == title && x.PlaningLevelId == levelId && x.PlanId == planId && (strategyObjectId == null || x.Id != strategyObjectId) && x.Deleted == false, ct);
    }

    public async Task<double> SumWeightByPerspectiveIdAsync(long perspectiveId, long? excludeStrategyObjectId, CancellationToken ct = default)
    {
        var values = await Db.Database.SqlQuery<double>($"""
            SELECT COALESCE(SUM(so.weight), 0.0) AS [Value]
            FROM dbo.tblstrategyobject so
            WHERE so.perspectid IN (
                SELECT p.perspectid FROM dbo.tblperspective p
                WHERE p.perspectid = COALESCE(
                    (SELECT selectedPerspective.parentid FROM dbo.tblperspective selectedPerspective
                     WHERE selectedPerspective.perspectid = {perspectiveId}), {perspectiveId})
                OR p.parentid = COALESCE(
                    (SELECT selectedPerspective.parentid FROM dbo.tblperspective selectedPerspective
                     WHERE selectedPerspective.perspectid = {perspectiveId}), {perspectiveId})
            )
            AND (so.isdeleted = 0 OR so.isdeleted IS NULL)
            AND ({excludeStrategyObjectId} IS NULL OR so.strategyobjectid <> {excludeStrategyObjectId})
            """).ToListAsync(ct);
        return values.Single();
    }
}
