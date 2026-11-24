#nullable enable
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.baseDomain.repository;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.model.dto;

namespace SemBackDotNet10.actionDomain.repository;

// CRUD implementation only; not the full translated Java query repository.
public sealed partial class ActionActPlanRepository(DbContext db)
    : SemRepository<ActionActPlan, long?>(db)
{
    public List<ActionActPlan> findAllWithTaskUrlIsNotNull()
        => Set.Where(x => x.TaskUrl != null && !x.IsDeleted).ToList();

    public List<ActionActPlan> findAllByActionPlanCatalogAndDeletedIsTrue(long catalogId)
        => Set.Where(x => x.ActionPlanCatalog != null
            && x.ActionPlanCatalog.Id == catalogId && x.IsDeleted).ToList();

    public ActionActPlan? FindByIdWithCatalogAndPeriod(long? id)
        => id is null ? null : Set.Include(x => x.ActionPlanCatalog)
            .Include(x => x.PlanPeriod).SingleOrDefault(x => x.Id == id.Value);

    // Projection mirrors Java findAllByActionPlanCatalog_Id's constructor arguments.
    public List<ActionActPlanDto> FindCorrectionRows(long catalogId, long planId)
        => Set.AsNoTracking().Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == catalogId && x.PlanPeriod != null
                && x.PlanPeriod.PlanId == planId && !x.IsDeleted)
            .OrderBy(x => x.PlanPeriod!.PlanId).ThenBy(x => x.Month)
            .Select(x => new ActionActPlanDto
            {
                Id = x.Id,
                PlanId = x.PlanPeriod!.PlanId,
                Month = x.Month,
                PlanValue = x.PlanValue,
                ActionPlanId = x.ActionPlanCatalog!.Id,
                ActualValue = x.ActualValue,
                IsPlanned = x.ActionPlanCatalog.IsPlanned,
                ActionPlanCatalogId = x.ActionPlanCatalog.Id,
                IsDeleted = x.IsDeleted,
                CorrectionActualValue = x.CorrectionActualValue,
                Correction = x.Correction,
                Deviation = x.Deviation
            }).ToList();

    // Synchronous bridge for the original void service contract. This saves every
    // tracked change in this DbContext, not just the rows modified by this service.
    public int CommitChanges() => Db.SaveChanges();

    protected override IOrderedQueryable<ActionActPlan> OrderForPaging(IQueryable<ActionActPlan> query)
        => query.OrderBy(x => x.Id);

    public ActionActPlan? FindById(long? id) => id is null ? null : Set.Find(id.Value);

    public List<ActionActPlan> findByActionPlanCatalogId(long? catalogId)
        => catalogId is null ? new List<ActionActPlan>() : Set
            .Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId)
            .OrderBy(x => x.Id).ToList();

    public List<ActionActPlan> findByActionPlanCatalogIdAndDeletedIsFalse(long? catalogId)
        => catalogId is null ? new List<ActionActPlan>() : Set
            .Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId && !x.IsDeleted)
            .OrderBy(x => x.Id).ToList();
}
