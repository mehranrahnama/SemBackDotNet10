namespace SemBackDotNet10.measureDomain.controller;

internal sealed record WeightedMeasureMonth(decimal? Plan, decimal? MonthPlan);
internal sealed record WeightedMeasureChild(decimal Weight, IReadOnlyDictionary<int, WeightedMeasureMonth> Months);
internal sealed record WeightedMeasureResult(decimal Plan, decimal MonthPlan);

internal static class MeasureWeightedPlanCalculator
{
    public static bool TryCalculate(IReadOnlyList<WeightedMeasureChild> children, IReadOnlySet<int> activeMonths,
        out Dictionary<int, WeightedMeasureResult> result)
    {
        result = new Dictionary<int, WeightedMeasureResult>();
        if (children.Count == 0 || children.Any(child => child.Months.Count != 12 ||
            Enumerable.Range(1, 12).Any(month => !child.Months.ContainsKey(month)))) return false;
        var weightSum = children.Sum(child => child.Weight == 0m ? 1m : child.Weight);
        if (weightSum == 0m) return false;
        foreach (var month in Enumerable.Range(1, 12))
        {
            if (!activeMonths.Contains(month))
            {
                result.Add(month, new WeightedMeasureResult(0m, 0m));
                continue;
            }
            if (children.Any(child => child.Months[month].Plan is null || child.Months[month].MonthPlan is null)) return false;
            var plan = children.Sum(child => child.Months[month].Plan!.Value * (child.Weight == 0m ? 1m : child.Weight));
            var monthPlan = children.Sum(child => child.Months[month].MonthPlan!.Value * (child.Weight == 0m ? 1m : child.Weight));
            result.Add(month, new WeightedMeasureResult(Math.Round(plan / weightSum, 4, MidpointRounding.AwayFromZero),
                Math.Round(monthPlan / weightSum, 4, MidpointRounding.AwayFromZero)));
        }
        return true;
    }
}
