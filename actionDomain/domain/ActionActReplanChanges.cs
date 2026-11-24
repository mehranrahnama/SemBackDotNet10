using System;
using System.Collections.Generic;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.domain;

/// <summary>Maps the monthly replan changes and audit rows from the Java workflow.
/// The caller must persist them inside the enclosing replan transaction.</summary>
public static class ActionActReplanChanges
{
    public static List<ActionActPlanLog> Apply(
        IEnumerable<ActionActReplanDto> changes,
        ActionPlanCatalog catalog,
        Func<long?, ActionActPlan> findPlan)
    {
        ArgumentNullException.ThrowIfNull(changes);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(findPlan);
        var logs = new List<ActionActPlanLog>();
        foreach (var change in changes)
        {
            var plan = findPlan(change.Id);
            var month = checked((int)(change.Month
                ?? throw new ArgumentException("Replan month is required.", nameof(changes))));
            logs.Add(new ActionActPlanLog
            {
                PlanPeriod = plan.PlanPeriod,
                PlanValueOld = change.PlanValue,
                ActualValueOld = change.ActualValue,
                PlanValueNew = change.PlanValueNew,
                ActualValueNew = change.ActualValueNew,
                ActionPlanCatalog = catalog,
                Month = month
            });
            if (change.PlanValueNew.HasValue) plan.PlanValue = change.PlanValueNew;
            if (change.ActualValueNew.HasValue) plan.ActualValue = change.ActualValueNew;
            plan.OutCome = change.ReplanOutCome;
        }
        return logs;
    }
}
