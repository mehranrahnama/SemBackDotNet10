#nullable enable
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;


namespace SemBackDotNet10.actionDomain.mapper;

public interface IActionActPlanMapper
{
    ActionActPlan? toEntity(ActionActPlanReplanDto? dto, PlanPeriod? planPeriod,
        ActionPlanCatalog? actionPlanCatalog);
    void copyPlanValueAndActualValue(ActionActPlan? source, ActionActPlan target);
    void copyDailyValueAndActualValue(ActionActPlan? source, ActionActPlan target);
    ActionActPlan? clone(ActionActPlan? source, ActionPlanCatalog? catalog);
    void copySupremeLevelValues(ActionActPlan? source, ActionActPlan existing);
    void rewriteTaskUrlEntityId(ActionActPlan source, ActionActPlan target);
}
