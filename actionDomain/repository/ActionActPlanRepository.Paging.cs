using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Common.utils;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.baseDomain.repository;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    internal IQueryable<ActionActPlan> CatalogRows(long catalogId, bool deleted)
        => Set.AsNoTracking().Where(x => x.ActionPlanCatalog != null
            && x.ActionPlanCatalog.Id == catalogId && x.PlanPeriod != null
            && x.IsDeleted == deleted);

    // The fixed Java ORDER BY remains primary. Requested sorting is appended,
    // matching Spring's treatment of a declared query order; ID breaks ties.
    internal static IQueryable<ActionActPlanDto> BuildCatalogPageQuery(
        IQueryable<ActionActPlan> rows, global::Common.utils.PageRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var offset = request.Offset;
        var ordered = rows.OrderBy(x => x.PlanPeriod!.PlanId).ThenBy(x => x.Month);
        var desc = request.Direction == SortDirection.Desc;
        ordered = request.OrderBy?.Trim().ToLowerInvariant() switch
        {
            null or "" => ordered,
            "id" => desc ? ordered.ThenByDescending(x => x.Id) : ordered.ThenBy(x => x.Id),
            "planid" or "planperiod.planid" => desc ? ordered.ThenByDescending(x => x.PlanPeriod!.PlanId) : ordered.ThenBy(x => x.PlanPeriod!.PlanId),
            "month" => desc ? ordered.ThenByDescending(x => x.Month) : ordered.ThenBy(x => x.Month),
            "planvalue" => desc ? ordered.ThenByDescending(x => x.PlanValue) : ordered.ThenBy(x => x.PlanValue),
            "actualvalue" => desc ? ordered.ThenByDescending(x => x.ActualValue) : ordered.ThenBy(x => x.ActualValue),
            _ => throw new ArgumentException("Unsupported catalog-page sort field.", nameof(request))
        };
        return ordered.ThenBy(x => x.Id).Skip(offset).Take(request.Size)
            .Select(x => new ActionActPlanDto
            {
                Id=x.Id, PlanId=x.PlanPeriod!.PlanId, Month=x.Month,
                PlanValue=x.PlanValue, ActionPlanId=x.Id, ActualValue=x.ActualValue,
                IsPlanned=x.ActionPlanCatalog!.IsPlanned,
                ActionPlanCatalogId=x.ActionPlanCatalog.Id
                // Eight-argument Java constructor leaves IsDeleted at false,
                // even for V2. Deliberately preserved; see README.
            });
    }

    public PageResult<ActionActPlanDto> FindCatalogPage(long catalogId,
        global::Common.utils.PageRequest request, bool deleted=false)
    {
        var rows = CatalogRows(catalogId, deleted);
        // Validate paging and sorting before issuing even the count query.
        var contentQuery = BuildCatalogPageQuery(rows, request);
        var total = rows.LongCount();
        var content = contentQuery.ToList();
        return new(content, total, request.Page, request.Size);
    }

    public PageResult<ActionActPlanDto> FindCatalogLevelPage(long catalogId, long? levelId,
        global::Common.utils.PageRequest request)
    {
        var rows = CatalogRows(catalogId, false).Where(x => x.ActionPlanCatalog!.PlaningLevel != null
            && x.ActionPlanCatalog.PlaningLevel.Id == levelId);
        var query = BuildCatalogPageQuery(rows, request);
        var total = rows.LongCount();
        return new(query.ToList(), total, request.Page, request.Size);
    }
}
