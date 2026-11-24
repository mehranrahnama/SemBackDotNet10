#nullable enable
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public List<ActionActPlan> FindPlansForCatalogCopy(long catalogId)
        => Set.Include(x => x.PlanPeriod).Include(x => x.CorrectionExecuter)
            .Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId && !x.IsDeleted)
            .ToList();
    // JPQL equality with any null parameter matches no row.
    // SingleOrDefault deliberately rejects duplicate matches (Spring Optional contract).
    public ActionActPlan? findByActionPlanCatalogIdAndMonthAndPlanPeriodAndDeletedFalse(
        long? actionPlanCatalogId, long? month, long? planId)
    {
        if (actionPlanCatalogId is null || month is null || planId is null)
            return null;

        return Set.Where(x => x.ActionPlanCatalog != null
            && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value
            && x.Month == month.Value
            && x.PlanPeriod != null
            && x.PlanPeriod.PlanId == planId.Value
            && !x.IsDeleted).SingleOrDefault();
    }
}
