#nullable enable
using System.Collections.Generic;
using System.Linq;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    // Source queries have no ORDER BY and no deleted predicate.
    public List<ActionActPlan> findByActionPlanCatalog_Id(long? id)
        => id is null ? new() : Set.Where(x => x.ActionPlanCatalog != null
            && x.ActionPlanCatalog.Id == id.Value).ToList();

    public List<ActionActPlan> findByActionCatalogId(long? actionCatalogId)
        => findByActionPlanCatalog_Id(actionCatalogId);

    public List<ActionActPlan> findAllByActionPlanCatalogAndDeletedIsFalse(
        long? actionPlanCatalogId, long? activePlanId)
        => actionPlanCatalogId is null || activePlanId is null ? new() : Set
            .Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value
                && x.PlanPeriod != null && x.PlanPeriod.PlanId >= activePlanId.Value
                && !x.IsDeleted).ToList();

    // Despite the name, the Java JPQL uses >= and includes deleted rows.
    public List<ActionActPlan> findByActionPlanCatalogIdAndGreaterThanPlanId(
        long? actionPlanCatalogId, long? activePlanId)
        => actionPlanCatalogId is null || activePlanId is null ? new() : Set
            .Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value
                && x.PlanPeriod != null && x.PlanPeriod.PlanId >= activePlanId.Value)
            .ToList();

    public ActionActPlan? findByIdAndDeletedIsFalse(long? actionActPlanId)
        => actionActPlanId is null ? null : Set
            .SingleOrDefault(x => x.Id == actionActPlanId.Value && !x.IsDeleted);
}
