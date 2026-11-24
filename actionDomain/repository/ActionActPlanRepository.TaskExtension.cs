using SemBackDotNet10.actionDomain.model.entity;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public void StageNewActionActPlan(ActionActPlan plan) => Set.Add(plan);
    public void StageRemoval(IEnumerable<ActionActPlan> plans) => Set.RemoveRange(plans);

    public void DeleteEvaluationsForPlans(IEnumerable<ActionActPlan> plans)
    {
        // The evaluation entity has not yet been mapped to EF Core. Use the
        // table and foreign key from the Java entity, within this context's transaction.
        foreach (var id in plans.Select(plan => plan.Id).Distinct())
            Db.Database.ExecuteSqlInterpolated(
                $"DELETE FROM eval.tblevaluationaction WHERE actionactplanid = {id}");
    }
}
