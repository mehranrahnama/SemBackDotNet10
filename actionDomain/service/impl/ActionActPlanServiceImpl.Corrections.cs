using System.Linq;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public ActionActPlan editDeviation(ActionActPlanDto dto)
    {
        ActionActPlan result = null!;
        _planningUnitOfWork.Execute(() =>
        {
            result = FindCorrectionPlan(dto.Id);
            result.Deviation = dto.Deviation;
            PropagateCorrection(result);
        });
        return result;
    }

    public ActionActPlanDto editCorrection(ActionActPlanDto dto)
    {
        ActionActPlanDto result = null!;
        _planningUnitOfWork.Execute(() =>
        {
            var plan = FindCorrectionPlan(dto.Id);
            if (dto.Correctionexecuter is null)
                throw new BusinessException("لطفا مجری را انتخاب نمایید");
            plan.Correction = dto.Correction;
            plan.CorrectionStartDateAction = dto.Correctionstartdateaction;
            plan.CorrectionDeadlineDate = dto.Correctiondeadlinedate;
            plan.CorrectionExecuter = _userService.FindByIdOrElseThrow(dto.Correctionexecuter.Value);
            plan.CorrectionActualValue = dto.CorrectionActualValue;
            plan.ActualValue = dto.ActualValue;
            plan.Deviation = dto.Deviation;
            PropagateCorrection(plan);
            result = mapToDto(plan);
        });
        return result;
    }

    private ActionActPlan FindCorrectionPlan(long? id)
        => _actionActPlanRepository.FindForCorrection(id)
            ?? throw new BusinessException("error.actionActPlan.idIsNotValid");

    private void PropagateCorrection(ActionActPlan source)
    {
        var ids = _actionPlanCatalogRelationService.findAncestorIds(source.ActionPlanCatalog!.Id)
            .Where(id => id.HasValue).Select(id => id!.Value).ToList();
        if (ids.Count == 0) return;
        var plans = _actionActPlanRepository.FindAncestorPlansForCorrection(
            ids, source.Month, source.PlanPeriod!.PlanId);
        foreach (var plan in plans)
        {
            plan.Deviation = source.Deviation;
            plan.Correction = source.Correction;
            plan.CorrectionStartDateAction = source.CorrectionStartDateAction;
            plan.CorrectionDeadlineDate = source.CorrectionDeadlineDate;
            plan.CorrectionExecuter = source.CorrectionExecuter;
            plan.CorrectionActualValue = source.CorrectionActualValue;
        }
    }
}
