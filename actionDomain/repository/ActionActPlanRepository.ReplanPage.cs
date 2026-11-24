using System.Linq;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.baseDomain.repository;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public PageResult<ActionActReplanDto> FindReplanPage(long catalogId,
        global::Common.utils.PageRequest request, bool deleted)
    {
        var rows = CatalogRows(catalogId, deleted);
        var total = rows.LongCount();
        var values = rows.OrderBy(x => x.PlanPeriod!.PlanId).ThenBy(x => x.Month)
            .ThenBy(x => x.Id).Skip(request.Offset).Take(request.Size)
            .Select(x => new { x.Id, PlanId = x.PlanPeriod!.PlanId, x.Month, x.PlanValue,
                x.ActualValue, x.ActionPlanCatalog!.IsReplanned, x.ActionPlanCatalog.ReplannedStatus })
            .ToList();
        var content = values.Select(x => new ActionActReplanDto
        {
            Id = x.Id, Year = x.PlanId.ToString().Substring(0, 4), Month = x.Month,
            PlanValue = x.PlanValue, ActualValue = x.ActualValue,
            IsReplanned = !deleted && x.IsReplanned,
            ReplannedStatus = deleted ? null : x.ReplannedStatus
        }).ToList();
        return new(content, total, request.Page, request.Size);
    }
}
