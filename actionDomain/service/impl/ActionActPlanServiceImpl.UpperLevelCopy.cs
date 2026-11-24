using System.Collections.Generic;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public void copyActionActPlansForUpperLevels(long? actionPlanCatalogId, List<long?> ancestorIds)
    {
        var basePlans = _actionActPlanRepository
            .findAllByActionPlanCatalogIdAndDeletedFalseSortedByPlanIdAndMonth(actionPlanCatalogId);

        foreach (var ancestorId in ancestorIds)
        {
            var ancestorPlans = _actionActPlanRepository
                .findAllByActionPlanCatalogIdAndDeletedFalseSortedByPlanIdAndMonth(ancestorId);

            // Java deliberately pairs by INDEX in the two ordered lists.
            // Do not replace with a key join, zip-to-shorter, padding or insertion.
            // A short ancestor list throws before persistence, as in the source.
            for (var i = 0; i < basePlans.Count; i++)
                _actionActPlanMapper.copyPlanValueAndActualValue(basePlans[i], ancestorPlans[i]);
        }

        // Both queries explicitly track rows, so no Update(graph) is necessary.
        // This is the single persistence boundary corresponding to Java saveAll.
        // Saves ALL pending context changes; caller owns an outer transaction.
        _actionActPlanRepository.CommitChanges();
    }
}
