using System.Collections.Generic;
using System.Linq;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public ActionActPlan? FindPreviousMonth(long catalogId, long? month, long planId)
        => Set.SingleOrDefault(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId
            && x.Month == month && x.PlanPeriod != null && x.PlanPeriod.PlanId == planId);

    public List<ActionActPlan> FindFutureMonths(long catalogId, long? month)
        => Set.Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId
            && x.Month > month && !x.IsDeleted).ToList();

    public decimal? GetFinalPlanValue(long catalogId, long planId)
        => Set.Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId
            && x.PlanPeriod != null && x.PlanPeriod.PlanId == planId && !x.IsDeleted)
            .Max(x => x.PlanValue);

    public List<ActionActPlan> FindFutureMonthsInPeriod(long catalogId, long? month, long planId)
        => Set.Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId
            && x.Month > month && !x.IsDeleted && x.PlanPeriod != null
            && x.PlanPeriod.PlanId == planId).ToList();
}
