using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.baseDomain.repository;
using SemBackDotNet10.Common.enums;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public PageResult<RelatedActsDto> FindRelatedValues(long? actId, global::Common.utils.PageRequest request)
    {
        var catalogIds = Set.Where(x => x.Id == actId && x.ActionPlanCatalog != null)
            .Select(x => x.ActionPlanCatalog!.Id);
        var rows = Set.AsNoTracking().Where(x => x.ActionPlanCatalog != null
            && catalogIds.Contains(x.ActionPlanCatalog.Id) && x.PlanPeriod != null);
        var total = rows.LongCount();
        var descending = request.Direction == SortDirection.Desc;
        var ordered = request.OrderBy?.Trim().ToLowerInvariant() switch
        {
            null or "" or "id" => descending ? rows.OrderByDescending(x => x.Id) : rows.OrderBy(x => x.Id),
            "month" => descending ? rows.OrderByDescending(x => x.Month) : rows.OrderBy(x => x.Month),
            "planvalue" => descending ? rows.OrderByDescending(x => x.PlanValue) : rows.OrderBy(x => x.PlanValue),
            "actualvalue" => descending ? rows.OrderByDescending(x => x.ActualValue) : rows.OrderBy(x => x.ActualValue),
            "planperiod.planid" => descending ? rows.OrderByDescending(x => x.PlanPeriod!.PlanId) : rows.OrderBy(x => x.PlanPeriod!.PlanId),
            _ => throw new ArgumentException("Unsupported related-value sort field.", nameof(request))
        };
        var data = ordered.ThenBy(x => x.Id).Skip(request.Offset).Take(request.Size)
            .Select(x => new { x.Id, x.Month, x.PlanValue, x.ActualValue, PlanId = x.PlanPeriod!.PlanId })
            .ToList().Select(x => new RelatedActsDto
            {
                Id = x.Id, Month = x.Month, PlanValue = x.PlanValue, ActualValue = x.ActualValue,
                Year = long.Parse(x.PlanId.ToString().Substring(0, 4))
            }).ToList();
        return new(data, total, request.Page, request.Size);
    }
}
