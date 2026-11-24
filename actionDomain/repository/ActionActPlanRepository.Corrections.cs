using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public ActionActPlan? FindForCorrection(long? id)
        => id is null ? null : Set.Include(x => x.ActionPlanCatalog)
            .Include(x => x.PlanPeriod).Include(x => x.CorrectionExecuter)
            .SingleOrDefault(x => x.Id == id.Value);

    public List<ActionActPlan> FindAncestorPlansForCorrection(
        List<long> catalogIds, long? month, long? planId)
        => Set.Where(x => x.ActionPlanCatalog != null
            && catalogIds.Contains(x.ActionPlanCatalog.Id)
            && x.Month == month && x.PlanPeriod != null
            && x.PlanPeriod.PlanId == planId && !x.IsDeleted).ToList();
}
