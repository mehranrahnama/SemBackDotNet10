using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    // JPQL bulk UPDATE equivalent: runs immediately, does not call SaveChanges,
    // and does not refresh tracked entities. Uses any existing DbContext transaction.
    public void updatePlanValuesToNullByActionPlanCatalogId(long? actionPlanCatalogId)
    {
        if (!actionPlanCatalogId.HasValue) return; // JPQL '= null' updates no rows.

        Set.Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == actionPlanCatalogId.Value && !x.IsDeleted)
            .ExecuteUpdate(update => update.SetProperty(x => x.PlanValue, (decimal?)null));
    }
}
