#nullable enable
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    // The Java name says GreaterThan, but its JPQL explicitly uses >=.
    // Keep that boundary inclusive; do not infer semantics from the method name.
    public List<ActionActPlan> findByActionPlanCatalogIdAndLevelIdGreaterThanActivePlanId(
        long? actionPlanCatalogId, long? levelId, long? activePlanId)
        => actionPlanCatalogId is null || levelId is null || activePlanId is null
            ? new List<ActionActPlan>()
            : BuildActivePlanBoundaryQuery(Set, actionPlanCatalogId.Value, levelId.Value,
                activePlanId.Value, atOrAfter: true).ToList();

    public List<ActionActPlan> findByActionPlanCatalogIdAndLevelIdLessThanActivePlanId(
        long? actionPlanCatalogId, long? levelId, long? activePlanId)
        => actionPlanCatalogId is null || levelId is null || activePlanId is null
            ? new List<ActionActPlan>()
            : BuildActivePlanBoundaryQuery(Set, actionPlanCatalogId.Value, levelId.Value,
                activePlanId.Value, atOrAfter: false).ToList();

    internal static IOrderedQueryable<ActionActPlan> BuildActivePlanBoundaryQuery(
        IQueryable<ActionActPlan> source, long catalogId, long levelId,
        long activePlanId, bool atOrAfter)
    {
        var query = source.Where(x => !x.IsDeleted
            && x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId
            && x.ActionPlanCatalog.PlaningLevel != null
            && x.ActionPlanCatalog.PlaningLevel.Id == levelId
            && x.PlanPeriod != null);
        query = atOrAfter
            ? query.Where(x => x.PlanPeriod!.PlanId >= activePlanId)
            : query.Where(x => x.PlanPeriod!.PlanId < activePlanId);
        // Managed entities are intentional: these Java query results feed write workflows.
        // Do not introduce AsNoTracking or a SaveChanges call here.
        return query.OrderBy(x => x.PlanPeriod!.PlanId).ThenBy(x => x.Month);
    }
}
