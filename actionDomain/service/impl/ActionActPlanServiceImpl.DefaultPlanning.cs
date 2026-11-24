using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public void removeDefaultPlanning(long? actionPlanCatalogId)
    {
        // Keep the real catalog service as the authority; do not infer this flag
        // from the existence or values of ActionActPlan rows.
        if (_actionPlanCatalogService.checkIsPlanned(actionPlanCatalogId))
            throw new BusinessException("امکان حذف کردن برنامه ریزی پیشفرض برای اقدام برنامه ریزی شده وجود ندارد");

        _actionActPlanRepository.updatePlanValuesToNullByActionPlanCatalogId(actionPlanCatalogId);
    }

}
