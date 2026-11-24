using System;
using System.Collections.Generic;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.domain;

/// <summary>
/// Computational portion extracted from ActionActPlanDomainService.java.
/// No database writes, no fabricated repository methods. Caller supplies chronological plans.
/// </summary>
public static class ActionActPlanPlanningCalculator
{
    public static List<ActionActPlan> CalculateDefaultPlanning(
        List<ActionActPlan> plans, decimal yearTarget, decimal yearStartValue)
    {
        ArgumentNullException.ThrowIfNull(plans);
        foreach (var plan in plans) ArgumentNullException.ThrowIfNull(plan);
        int firstYearMonths = 0;
        foreach (var plan in plans)
        {
            firstYearMonths++;
            if (plan.Month == 12) break;
        }
        CalculateMonthlyValues(plans.GetRange(0, firstYearMonths), yearTarget, yearStartValue);
        if (firstYearMonths < plans.Count)
            CalculateMonthlyValues(plans.GetRange(firstYearMonths, plans.Count - firstYearMonths),
                100m, yearTarget);
        return plans;
    }

    private static void CalculateMonthlyValues(List<ActionActPlan> plans, decimal target, decimal start)
    {
        if (plans.Count == 0) return;
        if (plans.Count == 1) { plans[0].PlanValue = target; return; }
        plans[0].PlanValue = start;
        int steps = plans.Count - 1;
        decimal difference = target - start;
        // Java uses BigDecimal.setScale(0, DOWN).intValue(). Here overflow is rejected,
        // rather than silently truncating a value outside Int32's supported range.
        int distributable = checked((int)decimal.Truncate(difference));
        int baseline = distributable / steps;
        int remainder = distributable % steps;
        decimal lastValue = start;
        for (int step = 0; step < steps; step++)
        {
            int increment = baseline + (step < remainder ? 1 : 0);
            decimal value = lastValue + increment;
            int index = step + 1;
            if (index == plans.Count - 1)
            {
                value += difference - distributable;
                plans[index].PlanValue = target;
            }
            else plans[index].PlanValue = decimal.Round(value, 0, MidpointRounding.AwayFromZero);
            lastValue = value;
        }
    }
}
