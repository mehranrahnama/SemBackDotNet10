namespace SemBackDotNet10.measureDomain.controller;

internal static class MeasureDefaultPlanCalculator
{
    public static bool TryGetActiveMonths(int period, bool alreadyPlanned, string? customMonth, out HashSet<int> activeMonths)
    {
        activeMonths = new HashSet<int>();
        if (period == 2 || period == 6 && alreadyPlanned) activeMonths.UnionWith(Enumerable.Range(1, 12));
        else if (period == 3) activeMonths.UnionWith(new[] { 3, 6, 9, 12 });
        else if (period == 4) activeMonths.UnionWith(new[] { 6, 12 });
        else if (period == 6)
        {
            if (string.IsNullOrWhiteSpace(customMonth)) return false;
            foreach (var part in customMonth.Split(';'))
                if (!int.TryParse(part, out var month) || month is < 1 or > 12 || !activeMonths.Add(month)) return false;
        }
        else activeMonths.Add(12);
        return true;
    }

    public static bool HasSymmetricMonths(IReadOnlySet<int> activeMonths) =>
        Enumerable.Range(1, 11).All(month => activeMonths.Contains(month) == activeMonths.Contains(12 - month));

    public static int DestinationMonth(int month, bool active, bool descending) =>
        descending && active && month != 12 ? 12 - month : month;

    public static (decimal? Plan, decimal? MonthPlan) Calculate(int month, bool active, decimal current, decimal target, int payeshType, bool keepState, bool descending = false)
    {
        if (!active) return (null, null);
        if (keepState) return (target, target);

        var step = payeshType switch
        {
            1 => Math.Round((target - current) / 12m, 4, MidpointRounding.AwayFromZero),
            2 => Math.Round((target - current) / 78m, 4, MidpointRounding.AwayFromZero),
            3 => Math.Round((target - current) * 12m / 78m, 4, MidpointRounding.AwayFromZero),
            _ => throw new ArgumentOutOfRangeException(nameof(payeshType))
        };
        if (descending && payeshType is 1 or 2) step = Math.Abs(step);
        if (descending)
        {
            var descendingMonthPlan = payeshType == 1 ? (month == 12 ? target : step * month + target) : step * month + current;
            var descendingSigma = month * (month + 1) / 2;
            var descendingPlan = month == 12 ? target : payeshType switch
            {
                1 => descendingMonthPlan,
                2 => step * descendingSigma + target,
                _ => Math.Round(step * descendingSigma / month, 4, MidpointRounding.AwayFromZero) + target
            };
            return (descendingPlan, descendingMonthPlan);
        }
        var monthPlan = payeshType == 1 && month == 12 ? target : step * month + current;
        var sigma = month * (month + 1) / 2;
        var plan = month == 12 ? target : payeshType switch
        {
            1 => monthPlan,
            2 => step * sigma + current,
            _ => Math.Round(step * sigma / month, 4, MidpointRounding.AwayFromZero) + current
        };
        return (plan, monthPlan);
    }
}
