using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public ActionPlanCatalogReplanDto? FindCatalogReplanData(long? catalogId)
    {
        if (catalogId is null) return null;
        var catalog = Db.Set<ActionPlanCatalog>().AsNoTracking()
            .Where(x => x.Id == catalogId && x.DeleteStatus == false && x.PlaningLevel != null)
            .Select(x => new ActionPlanCatalogReplanDto
            {
                LevelId = x.PlaningLevel!.Id, LevelDes = x.PlaningLevel.DesLevel,
                ActionPlanCatalogDescription = x.Description, PrevEndDate = x.EndDate,
                Replanned = x.IsReplanned, ReplannedStatus = x.ReplannedStatus
            }).SingleOrDefault();
        if (catalog == null) return null;
        var logs = Db.Set<ActionDateLog>().AsNoTracking()
            .Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId);
        var latestDate = logs.Max(x => x.ModifiedDate);
        if (latestDate != null)
        {
            // SingleOrDefault preserves the Java Optional query's duplicate-result failure.
            var latest = logs.Where(x => x.ModifiedDate == latestDate)
                .Select(x => new { x.EndDateNew, x.ReasonEndDateChange }).SingleOrDefault();
            if (latest != null)
            {
                catalog.NewEndDate = latest.EndDateNew;
                catalog.ReasonEndDateChange = latest.ReasonEndDateChange;
            }
        }
        return catalog;
    }

    public System.Collections.Generic.List<ActionActPlanReplanDto> FindActionActPlanReplanData(long? catalogId)
        => Set.AsNoTracking().Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId
                && !x.IsDeleted && x.PlanPeriod != null)
            .OrderBy(x => x.PlanPeriod!.PlanId).ThenBy(x => x.Month)
            .Select(x => new ActionActPlanReplanDto(x.Id, x.PlanPeriod!.PlanId, x.Month,
                x.PlanValue, null, x.ActualValue, null)).ToList();
}
