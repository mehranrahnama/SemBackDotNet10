using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

/// <summary>
/// Focused implementation of ActionPlanRepository.java's YearPcn bulk update.
/// Inject the SAME scoped action-domain context used by the encompassing workflow.
/// This is not an implementation of the full ActionPlanRepository interface.
/// </summary>
public sealed class ActionPlanYearPcnWriter(DbContext db)
{
    public void UpdateYearPcnByCatalogIdAndPlanId(
        long? catalogId, long? planId, decimal? yearStartPcn, decimal? yearCompletePcn)
    {
        // JPQL '= null' does not match rows. Do not translate it to IS NULL.
        if (catalogId is null || planId is null) return;

        // The source has NO deleted/status/strategy predicate.
        // Executes immediately; no SaveChanges, new transaction or tracker clearing.
        db.Set<ActionPlan>()
            .Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == catalogId.Value
                && x.PlanPeriod != null && x.PlanPeriod.PlanId == planId.Value)
            .ExecuteUpdate(update => update
                .SetProperty(x => x.YearStartPcn, yearStartPcn)
                .SetProperty(x => x.YearCompletePcn, yearCompletePcn));
    }
}
