using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public List<ActionActPlan> findAllByActionPlanCatalogIdAndDeletedFalseSortedByPlanIdAndMonth(
        long? actionPlanCatalogId)
    {
        if (actionPlanCatalogId is null) return new();

        // Tracked rows are required by the copy workflow. Explicitly load the
        // navigations read/copied by the mapper; do not depend on lazy loading.
        return Set.AsTracking()
            .Include(x => x.PlanPeriod)
            .Include(x => x.CorrectionExecuter)
            .Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value
                && !x.IsDeleted && x.PlanPeriod != null)
            .OrderBy(x => x.PlanPeriod!.PlanId)
            .ThenBy(x => x.Month)
            .ToList();
    }
}
