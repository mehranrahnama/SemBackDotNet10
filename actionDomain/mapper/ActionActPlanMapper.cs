#nullable enable
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;


namespace SemBackDotNet10.actionDomain.mapper;

// Explicit translation of ActionActPlanMapper.java with default MapStruct
// same-name property mapping (not an IgnoreByDefault mapper).
public sealed class ActionActPlanMapper : IActionActPlanMapper
{
    public ActionActPlan? toEntity(ActionActPlanReplanDto? dto,
        PlanPeriod? planPeriod, ActionPlanCatalog? actionPlanCatalog)
    {
        if (dto is null && planPeriod is null && actionPlanCatalog is null) return null;
        return new ActionActPlan
        {
            Month = dto?.Month,
            PlanValue = dto?.NewPlanValue,
            ActualValue = dto?.NewActualValue,
            PlanPeriod = planPeriod,
            ActionPlanCatalog = actionPlanCatalog
        };
    }

    public void copyPlanValueAndActualValue(ActionActPlan? source, ActionActPlan target)
    {
        if (source is null) return;
        ArgumentNullException.ThrowIfNull(target);
        CopyProperties(source, target, copyPeriodAndMonth: true, copyAuditAndVersion: true);
        rewriteTaskUrlEntityId(source, target);
    }

    public void copyDailyValueAndActualValue(ActionActPlan? source, ActionActPlan target)
    {
        if (source is null) return;
        ArgumentNullException.ThrowIfNull(target);
        CopyProperties(source, target, copyPeriodAndMonth: false, copyAuditAndVersion: true);
        rewriteTaskUrlEntityId(source, target);
    }

    public ActionActPlan? clone(ActionActPlan? source, ActionPlanCatalog? catalog)
    {
        if (source is null && catalog is null) return null;
        var target = new ActionActPlan { ActionPlanCatalog = catalog };
        if (source is not null)
        {
            CopyProperties(source, target, copyPeriodAndMonth: true, copyAuditAndVersion: false);
            rewriteTaskUrlEntityId(source, target);
        }
        return target;
    }

    public void copySupremeLevelValues(ActionActPlan? source, ActionActPlan existing)
    {
        if (source is null) return;
        ArgumentNullException.ThrowIfNull(existing);
        CopyProperties(source, existing, copyPeriodAndMonth: true, copyAuditAndVersion: false);
        rewriteTaskUrlEntityId(source, existing);
    }

    public void rewriteTaskUrlEntityId(ActionActPlan source, ActionActPlan target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);
        // Java Long id is null before persistence. The current C# entity uses
        // generated non-nullable long; zero is its unsaved sentinel.
        if (source.TaskUrl is null || target.Id == 0) return;
        target.TaskUrl = Regex.Replace(source.TaskUrl, @"entityId=[0-9]+",
            "entityId=" + target.Id.ToString(CultureInfo.InvariantCulture));
    }

    private static void CopyProperties(ActionActPlan source, ActionActPlan target,
        bool copyPeriodAndMonth, bool copyAuditAndVersion)
    {
        // Id and ActionPlanCatalog are ignored in all update mappings.
        target.PlanValue = source.PlanValue;
        target.ActualValue = source.ActualValue;
        target.ActualBudget = source.ActualBudget;
        target.TemporaryDelete = source.TemporaryDelete;
        target.DummyField = source.DummyField;
        target.Deviation = source.Deviation;
        target.Correction = source.Correction;
        target.CorrectionDeadlineDate = source.CorrectionDeadlineDate;
        target.CorrectionStartDateAction = source.CorrectionStartDateAction;
        target.CorrectionExecuter = source.CorrectionExecuter;
        target.CorrectionActualValue = source.CorrectionActualValue;
        target.EvalComment = source.EvalComment;
        target.EvalPcn = source.EvalPcn;
        target.EvalDesc = source.EvalDesc;
        target.TaskUrl = source.TaskUrl;
        target.IsDeleted = source.IsDeleted;
        target.RoyalVisionActId = source.RoyalVisionActId;
        target.ActionPCN = source.ActionPCN;
        target.Confirmstatus = source.Confirmstatus;
        target.DeleteFirstStatus = source.DeleteFirstStatus;
        target.ReplannedForHistory = source.ReplannedForHistory;
        target.ReplanStatusForHistory = source.ReplanStatusForHistory;
        target.OutCome = source.OutCome;

        if (copyPeriodAndMonth)
        {
            target.PlanPeriod = source.PlanPeriod;
            target.Month = source.Month;
        }
        if (copyAuditAndVersion)
        {
            target.Createdby = source.Createdby;
            target.Createddate = source.Createddate;
            target.Modifiedby = source.Modifiedby;
            target.Modifieddate = source.Modifieddate;
            target.Version = source.Version;
        }
    }
}
