using System;
using System.Collections.Generic;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.domain;

public static class ActionActPlanYearPcnCalculator
{
    // Input order is authoritative: Java uses the first/last row per PlanId,
    // not min/max values, and does not skip a null endpoint to find another one.
    // Tuples are private-to-this-migration calculation values, not an invented
    // replacement for the missing external Java YearPcn DTO.
    public static OrderedDictionary<long, (decimal YearStartPcn, decimal YearCompletePcn)>
        Calculate(IEnumerable<ActionActPlan> orderedPlans)
    {
        ArgumentNullException.ThrowIfNull(orderedPlans);
        var endpoints = new OrderedDictionary<long, (decimal? Start, decimal? End)>();
        foreach (var plan in orderedPlans)
        {
            if (plan?.PlanPeriod is null)
                throw new InvalidOperationException("اطلاعات دوره برنامه برای محاسبه مقادیر سالانه بارگذاری نشده است.");

            var planId = plan.PlanPeriod.PlanId;
            if (endpoints.TryGetValue(planId, out var current))
                endpoints[planId] = (current.Start, plan.PlanValue);
            else
                endpoints.Add(planId, (plan.PlanValue, plan.PlanValue));
        }

        var result = new OrderedDictionary<long, (decimal YearStartPcn, decimal YearCompletePcn)>();
        foreach (var entry in endpoints)
            if (entry.Value.Start.HasValue && entry.Value.End.HasValue)
                result.Add(entry.Key, (entry.Value.Start.Value, entry.Value.End.Value));
        return result;
    }
}
