using System;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    // Matches Java's intentionally limited DTO projection; does not set ActualValue/Budget.
    public ActionActPlanDto mapToDto(ActionActPlan actionActPlan)
    {
        ArgumentNullException.ThrowIfNull(actionActPlan);
        if (actionActPlan.PlanPeriod is null || actionActPlan.ActionPlanCatalog is null)
            throw new BusinessException("اطلاعات دوره یا کاتالوگ اقدام بارگذاری نشده است.");
        var executor = actionActPlan.CorrectionExecuter;
        return new ActionActPlanDto
        {
            Id = actionActPlan.Id,
            PlanId = actionActPlan.PlanPeriod.PlanId,
            Month = actionActPlan.Month,
            PlanValue = actionActPlan.PlanValue,
            ActionPlanCatalogId = actionActPlan.ActionPlanCatalog.Id,
            Deviation = actionActPlan.Deviation,
            Correction = actionActPlan.Correction,
            Correctionstartdateaction = actionActPlan.CorrectionStartDateAction,
            Correctiondeadlinedate = actionActPlan.CorrectionDeadlineDate,
            // Actual User.cs uses UserId, not Id.
            Correctionexecuter = executor?.UserId,
            ExecutorUserName = executor?.UserName,
            ExecutorDisplayName = executor is null ? null
                : $"{executor.NameF ?? "null"} {executor.FamilyF ?? "null"}",
            CorrectionActualValue = actionActPlan.CorrectionActualValue,
            RoyalVisionActId = actionActPlan.RoyalVisionActId
        };
    }

    // Returns Java query paths, NOT validated EF property names or SQL fragments.
    public string? mapOrderByToQueryOrderBy(string? orderBy) => orderBy switch
    {
        "id" => "id",
        "hadafName" => "strategyObject.title",
        "payeshgarName" => "payeshgar.nameF",
        "actionPlanCatalogDescription" => "actionPlanCatalog.description",
        _ => "id"
    };
}
