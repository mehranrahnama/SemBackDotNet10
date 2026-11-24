using System.Linq;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public (decimal? Plan, decimal? Actual) GetFinalValues(long catalogId)
    {
        var rows = Set.Where(x => x.ActionPlanCatalog != null && x.ActionPlanCatalog.Id == catalogId);
        return (rows.Max(x => x.PlanValue), rows.Max(x => x.ActualValue));
    }
}
