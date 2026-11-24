#nullable enable
using System.Collections.Generic;
using System.Linq;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public List<ActionActPlan> findByPlanIdAndMonthBetweenAndActualValueIsNotNull(
        long? planId, long? fromMonth, long? toMonth,
        List<long?>? userLevelIds, bool? isAdmin)
    {
        if (planId is null || fromMonth is null || toMonth is null)
            return new();

        var rows = BuildMonthlyReportRows(planId.Value, userLevelIds, isAdmin == true);
        return rows.Where(x => x.Month >= fromMonth.Value && x.Month <= toMonth.Value)
            .ToList();
    }

    public List<ActionActPlan> findByPlanIdAndMonthBetweenAndActualValueIsNotNull_Hosco(
        long? planId, long? fromMonth, long? toMonth,
        List<long?>? userLevelIds, bool? isAdmin)
    {
        if (planId is null) return new();

        var rows = BuildMonthlyReportRows(planId.Value, userLevelIds, isAdmin == true);
        // The correlated MAX uses the same catalog/year, not the supplied month range.
        // Null bounds disable BETWEEN only; they must not disable the MAX alternative.
        return rows.Where(x =>
            (fromMonth.HasValue && toMonth.HasValue
                && x.Month >= fromMonth.Value && x.Month <= toMonth.Value)
            || (x.Month.HasValue && x.Month == Set
                .Where(last => last.PlanPeriod != null
                    && last.PlanPeriod.PlanId == planId.Value
                    && !last.IsDeleted
                    && last.ActionPlanCatalog != null
                    && last.ActionPlanCatalog.IsPlanned
                    && last.Confirmstatus != 50L
                    && last.ActionPlanCatalog.Id == x.ActionPlanCatalog!.Id)
                .Max(last => last.Month)))
            .ToList();
    }

    private IQueryable<ActionActPlan> BuildMonthlyReportRows(
        long planId, List<long?>? userLevelIds, bool isAdmin)
    {
        var rows = Set.Where(x => x.PlanPeriod != null
            && x.PlanPeriod.PlanId == planId
            && !x.IsDeleted
            && x.ActionPlanCatalog != null
            && x.ActionPlanCatalog.IsPlanned
            && x.Confirmstatus != 50L);

        if (isAdmin) return rows;

        var ids = userLevelIds?.Where(x => x.HasValue)
            .Select(x => x!.Value).Distinct().ToArray() ?? System.Array.Empty<long>();
        // Fail closed when a non-admin has no usable planning-level ids.
        return rows.Where(x => x.ActionPlanCatalog!.PlaningLevel != null
            && ids.Contains(x.ActionPlanCatalog.PlaningLevel.Id));
    }
}
