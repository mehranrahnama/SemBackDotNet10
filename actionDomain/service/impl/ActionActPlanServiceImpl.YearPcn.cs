using System.Collections.Generic;
using SemBackDotNet10.actionDomain.domain;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    private OrderedDictionary<long, (decimal YearStartPcn, decimal YearCompletePcn)>
        calculateYearPcnFromPlanValues(long? actionPlanCatalogId)
        => ActionActPlanYearPcnCalculator.Calculate(
            _actionActPlanRepository.findActionPlansByCatalogOrderByPlanIdAscMonthAsc(actionPlanCatalogId));

    private void applyYearPcn(long? catalogId,
        OrderedDictionary<long, (decimal YearStartPcn, decimal YearCompletePcn)> yearPcnMap)
    {
        // Preserve Java LinkedHashMap iteration order and its per-year commands.
        foreach (var (planId, yearPcn) in yearPcnMap)
            _actionPlanDomainService.syncYearPcnByPlanId(
                catalogId, planId, yearPcn.YearStartPcn, yearPcn.YearCompletePcn);
    }
}
