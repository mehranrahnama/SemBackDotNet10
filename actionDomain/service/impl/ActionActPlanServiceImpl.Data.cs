using System.Collections.Generic;
using SemBackDotNet10.actionDomain.model.dto;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public HashSet<ActionActPlanDataDto> getActionActPlan(long? actionPlanCatalogId)
        => _actionActPlanRepository.FindCatalogData(actionPlanCatalogId);
}
