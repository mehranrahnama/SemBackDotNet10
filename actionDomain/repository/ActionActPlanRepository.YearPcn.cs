using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    internal static IQueryable<ActionActPlan> BuildYearPcnPlansQuery(
        IQueryable<ActionActPlan> rows, long catalogId)
        => rows.Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == catalogId && !x.IsDeleted
                && x.PlanPeriod != null)
            .Include(x => x.PlanPeriod)
            .OrderBy(x => x.PlanPeriod!.PlanId).ThenBy(x => x.Month);

    public List<ActionActPlan> findActionPlansByCatalogOrderByPlanIdAscMonthAsc(long? catalogId)
        => catalogId.HasValue
            ? BuildYearPcnPlansQuery(Set.AsNoTracking(), catalogId.Value).ToList()
            : new List<ActionActPlan>();
}
