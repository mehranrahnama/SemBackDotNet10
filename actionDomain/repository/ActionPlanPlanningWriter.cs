#nullable enable
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.domain;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

/// <summary>Planning queries and tracked mutations; the caller owns flush and transaction.</summary>
public sealed class ActionPlanPlanningWriter(DbContext db)
{
    public bool findAddToSupremeLevelByActionPlanCatalogIdAndStrategyObjectId(long? actionPlanCatalogId)
    {
        // The Java query has no strategy-object parameter despite its method name.
        return db.Set<ActionPlan>().Any(x => actionPlanCatalogId != null
            && x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value
            && x.DeleteStatus == false && x.AddToSupremeLevel == true);
    }

    public List<ActionPlan> findByActionPlanCatalogIdAndStrategyObjId(
        long? actionPlanCatalogId, long? strategyObjectId)
    {
        return db.Set<ActionPlan>().AsTracking()
            .Where(x => actionPlanCatalogId != null && strategyObjectId != null
                && x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value
                && x.StrategyObject != null && x.StrategyObject.Id == strategyObjectId.Value
                && x.DeleteStatus == false)
            .ToList();
    }

    // Status resolution remains the service's responsibility. Never infer status constants here.
    public void StageConfirmStatusAndTaskUrlAfterPlanning(long? actionPlanCatalogId,
        long? strategyObjectId, long? newStatus, long? oldStatus, string outcome,
        ActionTaskUrlBuilder taskUrls)
    {
        foreach (var actionPlan in findByActionPlanCatalogIdAndStrategyObjId(
            actionPlanCatalogId, strategyObjectId))
        {
            var taskUrl = taskUrls.generateTaskUrl(actionPlan.Id, newStatus, oldStatus);
            actionPlan.TaskUrl = taskUrl;
            actionPlan.Confirmstatus = newStatus;
            actionPlan.OutCome = outcome;
        }
    }
}
