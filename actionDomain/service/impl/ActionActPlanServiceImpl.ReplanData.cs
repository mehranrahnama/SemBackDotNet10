using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public ActionPlanCatalogReplanDto loadReplanDataByActionPlanCatalog(long? actionPlanCatalogId)
    {
        var result = _actionActPlanRepository.FindCatalogReplanData(actionPlanCatalogId)
            ?? throw new BusinessException("رکوردی برای شناسه کاتالوگ اقدام وارد شده پیدا نشد!");
        result.ActionActPlanList = _actionActPlanRepository.FindActionActPlanReplanData(actionPlanCatalogId);
        return result;
    }
}
