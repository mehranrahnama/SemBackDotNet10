using System.Globalization;
using System.Text.RegularExpressions;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.mapper;

public sealed class ActionPlanCatalogMapper : IActionPlanCatalogMapper
{
    public void copyValues(ActionPlanCatalog catalog, ActionPlanCatalog baseCatalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        if (baseCatalog is null) return;
        CopyCommon(catalog, baseCatalog);
        catalog.Payeshgar = baseCatalog.Payeshgar;
        catalog.Modir = baseCatalog.Modir;
        RewriteTaskUrl(catalog, baseCatalog);
    }

    public ActionPlanCatalog setTaskUrl(ActionPlanCatalog target, ActionPlanCatalog source)
    {
        ArgumentNullException.ThrowIfNull(target);
        if (source is null) return target;
        CopyCommon(target, source);
        RewriteTaskUrl(target, source);
        return target;
    }

    private static void CopyCommon(ActionPlanCatalog target, ActionPlanCatalog source)
    {
        target.Description = source.Description;
        target.StartDate = source.StartDate;
        target.EndDate = source.EndDate;
        target.TotalBudget = source.TotalBudget;
        target.Code = source.Code;
        target.OriginId = source.OriginId;
        target.Confirmstatus = source.Confirmstatus;
        target.AddToSupremeLevel = source.AddToSupremeLevel;
        target.DeleteStatus = source.DeleteStatus;
        target.TaskUrl = source.TaskUrl;
        target.IsPlanned = source.IsPlanned;
        target.ReplannedStatus = source.ReplannedStatus;
        target.IsReplanned = source.IsReplanned;
        target.CaseInstanceId = source.CaseInstanceId;
        target.BusinessKey = source.BusinessKey;
        target.ProcessId = source.ProcessId;
        target.DefinitionId = source.DefinitionId;
        target.Createdby = source.Createdby;
        target.Modifiedby = source.Modifiedby;
        target.Createddate = source.Createddate;
        target.Modifieddate = source.Modifieddate;
        target.Version = source.Version;
        target.OutCome = source.OutCome;
        target.DeleteFirstStatus = source.DeleteFirstStatus;
        target.ReplanStatusForHistory = source.ReplanStatusForHistory;
        target.ReplannedForHistory = source.ReplannedForHistory;
        target.ActionPlans = source.ActionPlans is null ? null : new List<ActionPlan>(source.ActionPlans);
        target.ActionActPlans = source.ActionActPlans is null ? null : new List<ActionActPlan>(source.ActionActPlans);
    }

    private static void RewriteTaskUrl(ActionPlanCatalog target, ActionPlanCatalog source)
    {
        if (source.TaskUrl is not null && target.Id != 0)
            target.TaskUrl = Regex.Replace(source.TaskUrl, @"entityId=[0-9]+",
                "entityId=" + target.Id.ToString(CultureInfo.InvariantCulture));
    }
}
