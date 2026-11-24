using System;
using System.Collections.Generic;
using System.Globalization;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.domain;

/// <summary>Port of Java setPreviousValue. Input order and object identity matter.</summary>
public static class ActionActPlanPreviousValueCalculator
{
    public static List<ActionActPlanDto> Calculate(List<ActionActPlanDto> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        // Validate before mutation, so an invalid later row cannot partially change
        // earlier DTOs. No sorting or inferred historical data is introduced.
        foreach (var row in rows)
        {
            if (row is null || row.ActionPlanCatalogId is null || row.Month is null
                || row.PlanId is null
                || row.PlanId.Value.ToString(CultureInfo.InvariantCulture).Length < 4)
                throw new BusinessException("اطلاعات دوره، ماه یا کاتالوگ اقدام معتبر نیست.");
        }

        var result = new List<ActionActPlanDto>();
        var seen = new HashSet<ActionActPlanDto>(ReferenceEqualityComparer.Instance);
        void Add(ActionActPlanDto row) { if (seen.Add(row)) result.Add(row); }
        for (var i = 0; i < rows.Count; i++)
        {
            var current = rows[i];
            current.Year = current.PlanId!.Value.ToString(CultureInfo.InvariantCulture)[..4];
            for (var j = i + 1; j < rows.Count; j++)
            {
                var next = rows[j];
                if (current.ActionPlanCatalogId != next.ActionPlanCatalogId
                    || current.Month != next.Month) continue;
                if (current.IsDeleted)
                {
                    next.PreviousPlanValue = current.PlanValue;
                    Add(next);
                }
                else
                {
                    current.PreviousPlanValue = next.PlanValue;
                    Add(current);
                }
            }
            if (!current.IsDeleted) Add(current);
        }
        return result;
    }
}
