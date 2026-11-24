#nullable enable
using System.Collections.Generic;
using System.Linq;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    // Overload of the existing long catalogId method, matching the Java entity argument.
    // Spring derived equality with a null argument is IS NULL, not '= NULL'.
    public List<ActionActPlan> findAllByActionPlanCatalogAndDeletedIsTrue(
        ActionPlanCatalog? actionPlanCatalog)
    {
        var rows = Set.Where(x => x.IsDeleted);
        if (actionPlanCatalog is null)
            return rows.Where(x => x.ActionPlanCatalog == null).ToList();

        var catalogId = actionPlanCatalog.Id;
        return rows.Where(x => x.ActionPlanCatalog != null
            && x.ActionPlanCatalog.Id == catalogId).ToList();
    }

    public List<ActionActPlan> findByActionPlanCatalogAndPlanPeriodAndMonthAndDeletedIsFalse(
        ActionPlanCatalog? actionPlanCatalog, PlanPeriod? planPeriod, long? month)
    {
        var rows = Set.Where(x => !x.IsDeleted);
        if (actionPlanCatalog is null)
            rows = rows.Where(x => x.ActionPlanCatalog == null);
        else
        {
            var catalogId = actionPlanCatalog.Id;
            rows = rows.Where(x => x.ActionPlanCatalog != null
                && x.ActionPlanCatalog.Id == catalogId);
        }

        if (planPeriod is null)
            rows = rows.Where(x => x.PlanPeriod == null);
        else
        {
            // Entity equality uses the internal PK, not the business PlanId.
            var periodId = planPeriod.Id;
            rows = rows.Where(x => x.PlanPeriod != null && x.PlanPeriod.Id == periodId);
        }

        return month is null
            ? rows.Where(x => x.Month == null).ToList()
            : rows.Where(x => x.Month == month.Value).ToList();
    }
}
