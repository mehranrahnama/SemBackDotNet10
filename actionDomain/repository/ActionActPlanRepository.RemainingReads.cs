#nullable enable
using System.Collections.Generic;
using System.Linq;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public List<ActionActPlan> findByActionPlanCatalog_IdAndMonthGreaterThanAndDeletedFalse(
        long? actionPlanCatalogId, long? month)
        => actionPlanCatalogId is null || month is null ? new() : Set
            .Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value
                && x.Month > month.Value && !x.IsDeleted)
            .ToList();

    public ActionActPlan? findByActionPlanCatalogIdAndMonthAndPlanId(
        long? actionPlanCatalogId, long? month, long? planId)
        => actionPlanCatalogId is null || month is null || planId is null ? null : Set
            .Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value
                && x.Month == month.Value && !x.IsDeleted
                && x.PlanPeriod != null && x.PlanPeriod.PlanId == planId.Value)
            .SingleOrDefault();

    public List<ActionActPlan> findAllByActionPlanCatalogIdInAndMonthAndPlanIdAndDeletedFalse(
        List<long?>? actionPlanCatalogIds, long? month, long? planId)
    {
        if (actionPlanCatalogIds is null || month is null || planId is null)
            return new();
        var ids = actionPlanCatalogIds.Where(x => x.HasValue)
            .Select(x => x!.Value).Distinct().ToArray();
        if (ids.Length == 0) return new();

        return Set.Where(x => x.ActionPlanCatalog != null
            && ids.Contains(x.ActionPlanCatalog.Id)
            && x.Month == month.Value
            && x.PlanPeriod != null && x.PlanPeriod.PlanId == planId.Value
            && !x.IsDeleted).ToList();
    }

    // Intentionally includes deleted rows: this JPQL has no deleted predicate.
    public List<ActionActPlan> findAllByActionPlanCatalogIdsAndMonth(
        HashSet<long?>? catalogIds, long? month, long? planId)
    {
        if (catalogIds is null || month is null || planId is null)
            return new();
        var ids = catalogIds.Where(x => x.HasValue)
            .Select(x => x!.Value).ToArray();
        if (ids.Length == 0) return new();

        return Set.Where(x => x.ActionPlanCatalog != null
            && ids.Contains(x.ActionPlanCatalog.Id)
            && x.Month == month.Value
            && x.PlanPeriod != null && x.PlanPeriod.PlanId == planId.Value)
            .ToList();
    }

    public List<ActionActPlanReplanDto> findActionActPlanReplanDtoByActionPlanCatalogId(
        long? actionPlanCatalogId)
        => actionPlanCatalogId is null ? new() : Set
            .Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value
                && !x.IsDeleted && x.PlanPeriod != null)
            .OrderBy(x => x.PlanPeriod!.PlanId).ThenBy(x => x.Month)
            .Select(x => new ActionActPlanReplanDto(
                x.Id, x.PlanPeriod!.PlanId, x.Month,
                x.PlanValue, null, x.ActualValue, null))
            .ToList();
}
