using System.Collections.Generic;
using SemBackDotNet10.actionDomain.domain;
using SemBackDotNet10.actionDomain.model.dto;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public List<ActionActPlanDto> findAllActionActPlansOfActionPlan(long? actionActPlanId)
    {
        // V7 performs explicit navigation loading and the original catalog/period
        // projection. Do not use mapToDto here: its shape differs from this query.
        var rows = getCorrectionByActPlanId(actionActPlanId);
        return ActionActPlanPreviousValueCalculator.Calculate(rows);
    }
}
