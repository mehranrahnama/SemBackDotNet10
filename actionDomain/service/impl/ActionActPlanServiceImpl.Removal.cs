using System.Collections.Generic;
using System.Linq;
using Common.enums;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public void removeExpiredActionActPlan(List<ActionActPlan> actionActPlans)
    {
        _planningUnitOfWork.Execute(() =>
        {
            _actionActPlanRepository.DeleteEvaluationsForPlans(actionActPlans);
            RemovePlansAndAttachments(actionActPlans);
        });
    }

    private void RemovePlansAndAttachments(List<ActionActPlan> plans)
    {
        if (plans.Count == 0) return;
        _storedAttachmentService.DeleteByResourceAndEntityIds(
            EntityNames.ACTION_ACT_PLAN, plans.Select(x => x.Id).ToList());
        _actionActPlanRepository.StageRemoval(plans);
    }

    public void removeActiveActionActPlans(long? actionPlanCatalogId, long? activePlanId)
    {
        _planningUnitOfWork.Execute(() => RemovePlansAndAttachments(
            _actionActPlanRepository.findAllByActionPlanCatalogAndDeletedIsFalse(actionPlanCatalogId, activePlanId)));
    }

    public void deleteActionPlanCatalog(long? actionPlanCatalogId)
    {
        _planningUnitOfWork.Execute(() => RemovePlansAndAttachments(
            _actionActPlanRepository.findByActionPlanCatalog_Id(actionPlanCatalogId)));
    }

    public void restoreActionActPlans(ActionPlanCatalog actionPlanCatalog, long? activePlanId)
    {
        _planningUnitOfWork.Execute(() =>
        {
            RemovePlansAndAttachments(_actionActPlanRepository
                .findAllByActionPlanCatalogAndDeletedIsFalse(actionPlanCatalog.Id, activePlanId));
            var deleted = _actionActPlanRepository.findAllByActionPlanCatalogAndDeletedIsTrue(actionPlanCatalog.Id);
            foreach (var plan in deleted) plan.IsDeleted = false;
        });
    }
}
