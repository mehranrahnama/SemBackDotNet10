using System;
using System.Collections.Generic;
using System.Globalization;

namespace SemBackDotNet10.Common.utils;

/// <summary>Port of CalculatorHelpers.java. Dates are year/month[/day], not Gregorian conversions.</summary>
public static class CalculatorHelpers
{
    public static List<int[]> calculateNumberOfMonthsBetween(string startDate, string endDate)
    {
        var (year, month) = ParseYearMonth(startDate);
        var (endYear, endMonth) = ParseYearMonth(endDate);
        int count = checked(12 * (endYear - year) + endMonth - month);
        var result = new List<int[]>();
        for (int i = 0; i <= count; i++)
        {
            result.Add(new[] { year, month });
            if (month == 12) { month = 1; year = checked(year + 1); }
            else month++;
        }
        return result;
    }

    public static List<long> calculateYearsBetween(string startDate, string endDate)
    {
        int start = ParseYearMonth(startDate).Year;
        int end = ParseYearMonth(endDate).Year;
        var result = new List<long>();
        for (long year = start; year <= end; year++) result.Add(checked(year * 100 + 1));
        return result;
    }

    public static bool dateHasChanged(long? firstYear, long? lastYear,
        long? firstMonth, long? lastMonth, string[] startDate, string[] endDate)
    {
        ArgumentNullException.ThrowIfNull(startDate);
        ArgumentNullException.ThrowIfNull(endDate);
        var start = ParseYearMonth(string.Join("/", startDate));
        var end = ParseYearMonth(string.Join("/", endDate));
        // As in Java, firstYear/lastYear are year numbers, not encoded PlanIds.
        return firstYear != start.Year || lastYear != end.Year
            || firstMonth != start.Month || lastMonth != end.Month;
    }

    private static (int Year, int Month) ParseYearMonth(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        string[] parts = value.Split('/');
        if (parts.Length < 2 || !int.TryParse(parts[0], NumberStyles.Integer,
                CultureInfo.InvariantCulture, out int year)
            || !int.TryParse(parts[1], NumberStyles.Integer,
                CultureInfo.InvariantCulture, out int month)
            || year < 1 || month < 1 || month > 12)
            throw new FormatException("Expected a positive year and month 1..12: yyyy/MM[/dd].");
        return (year, month);
    }
}
