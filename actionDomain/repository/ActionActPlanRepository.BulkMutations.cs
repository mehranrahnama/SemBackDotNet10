#nullable enable
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    // JPQL bulk operations execute immediately, participating in the current transaction.
    // They neither SaveChanges nor synchronize tracked entities.
    public void updateActionActPlanActualValue(
        decimal? actualValue, long? month, long? planId, long? catalogId)
    {
        if (month is null || planId is null || catalogId is null) return;

        // No deleted predicate in the source JPQL: do not silently add one.
        Set.Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == catalogId.Value
                && x.Month == month.Value
                && x.PlanPeriod != null && x.PlanPeriod.PlanId == planId.Value)
            .ExecuteUpdate(update => update.SetProperty(x => x.ActualValue, actualValue));
    }

    public void softDeleteAllByActionPlanCatalogId(long? actionPlanCatalogId)
    {
        if (actionPlanCatalogId is null) return;

        Set.Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value)
            .ExecuteUpdate(update => update.SetProperty(x => x.IsDeleted, true));
    }

    public void restoreRemovedActionActPlanByActionPlanCatalog(long? actionPlanCatalogId)
    {
        if (actionPlanCatalogId is null) return;

        Set.Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value && x.IsDeleted)
            .ExecuteUpdate(update => update.SetProperty(x => x.IsDeleted, false));
    }

    public void deleteAllExpiredRecordsByActionPlanCatalogId(long? actionPlanCatalogId)
    {
        if (actionPlanCatalogId is null) return;

        Set.Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value && x.IsDeleted)
            .ExecuteDelete();
    }

    public void removeActiveActionActPlansByActionPlanCatalog(long? actionPlanCatalogId)
    {
        if (actionPlanCatalogId is null) return;

        // Physical DELETE, not soft delete: matches the source JPQL exactly.
        Set.Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value && !x.IsDeleted)
            .ExecuteDelete();
    }
}
