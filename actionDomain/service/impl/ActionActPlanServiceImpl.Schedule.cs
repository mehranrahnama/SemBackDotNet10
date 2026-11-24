using System.Collections.Generic;
using SemBackDotNet10.actionDomain.model.dto;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public void changeActionActPlanSchedule(List<ActionActPlanDto> actionActPlanDtoList)
    {
        _planningUnitOfWork.Execute(() =>
        {
            foreach (var dto in actionActPlanDtoList)
            {
                var plan = findByIdOrElseThrow(dto.Id);
                plan.PlanValue = dto.PlanValueNew;
                plan.ActualValue = dto.ActualValueNew;
                plan.Month = dto.Month;
                plan.Deviation = dto.Deviation;
                plan.Correction = dto.Correction;
                plan.ActualBudget = dto.ActualBudget;
                plan.ActionPlanCatalog = _actionPlanCatalogService.findByIdOrElseThrow(dto.ActionPlanCatalogId);
            }

            _actionActPlanRepository.CommitChanges();
        });
    }
}
