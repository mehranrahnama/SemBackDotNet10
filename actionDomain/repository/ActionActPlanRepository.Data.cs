using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    // Separate from V9 paging: no ORDER BY and no pagination in the Java query.
    internal static IQueryable<ActionActPlanDataDto> BuildCatalogDataQuery(
        IQueryable<ActionActPlan> rows, long? catalogId)
        => rows.Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == catalogId
                && x.PlanPeriod != null && !x.IsDeleted)
            .Select(x => new ActionActPlanDataDto(
                x.ActionPlanCatalog!.Id, x.PlanPeriod!.PlanId, x.Month,
                x.PlanValue, x.Id, x.ActionPlanCatalog.IsPlanned));

    public HashSet<ActionActPlanDataDto> FindCatalogData(long? catalogId)
    {
        // JPQL '= :null' matches no rows. Do not reinterpret null as an unfiltered search.
        if (!catalogId.HasValue) return new HashSet<ActionActPlanDataDto>();
        // HashSet materialization is client-side, after server-side filtering/projection.
        return BuildCatalogDataQuery(Set.AsNoTracking(), catalogId).ToHashSet();
    }
}
