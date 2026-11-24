using System.Collections.Generic;
using System.Linq;
using Common.enums;
using SemBackDotNet10.actionDomain.model.dto;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    private void AddGridActivities(List<ActionProgressDto> progress)
    {
        if (progress.Count == 0) return;
        var catalogIds = progress.Where(x => x.ActionPlanCatalogId.HasValue)
            .Select(x => x.ActionPlanCatalogId!.Value).Distinct().ToList();
        var activities = _actionActPlanRepository.FindGridActivities(catalogIds);
        var entityIds = activities.Where(x => x.EntityId != null).Select(x => x.EntityId!).ToList();
        var attachments = _storedAttachmentRepository.FindByEntityIdIn(EntityNames.ACTION_PLAN_ACTIVITY, entityIds)
            .Where(x => x.EntityId != null).ToLookup(x => x.EntityId!);
        foreach (var activity in activities)
        {
            activity.Attachments = activity.EntityId == null ? new() : attachments[activity.EntityId].ToList();
            activity.HasUploadedFile = activity.Attachments.Count > 0;
        }
        var byCatalog = activities.ToLookup(x => x.ActionPlanCatalogId);
        foreach (var item in progress)
            item.Activities = byCatalog[item.ActionPlanCatalogId].ToList();
    }

    private void SetGridUploadedFiles(List<ActionProgressDto> progress)
    {
        var ids = progress.Where(x => x.ActionActPlanId.HasValue)
            .Select(x => x.ActionActPlanId!.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToList();
        var uploaded = _storedAttachmentRepository.FindEntityIdsWithFile(EntityNames.ACTION_ACT_PLAN, ids).ToHashSet();
        foreach (var item in progress)
            item.HasUploadedFile = item.ActionActPlanId.HasValue && uploaded.Contains(
                item.ActionActPlanId.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }
}
