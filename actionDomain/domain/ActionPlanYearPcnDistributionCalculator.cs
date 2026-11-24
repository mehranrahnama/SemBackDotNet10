using System;
using System.Collections.Generic;
using System.Numerics;

namespace SemBackDotNet10.actionDomain.domain;

/// <summary>Pure port of ActionPlanDomainService.calculateYearPcnByPlanId.
/// Not a replacement domain service or a persistence implementation.</summary>
public static class ActionPlanYearPcnDistributionCalculator
{
    public static OrderedDictionary<long, (decimal YearStartPcn, decimal YearCompletePcn)> Calculate(
        int startYear, int endYear, int currentYear,
        decimal? currentYearStartPcn, decimal? currentYearCompletePcn,
        Func<Exception> inconsistentYearPlanData)
    {
        ArgumentNullException.ThrowIfNull(inconsistentYearPlanData);
        var result = new OrderedDictionary<long, (decimal YearStartPcn, decimal YearCompletePcn)>();
        if (endYear < startYear) return result;
        if (currentYear < startYear || currentYear > endYear || currentYearCompletePcn is null)
            throw inconsistentYearPlanData();

        // long avoids accidental intermediate int overflow in the extracted calculation.
        long before = (long)currentYear - startYear;
        long after = (long)endYear - currentYear;
        if (before == 0 && after == 0)
        {
            result.Add(PlanId(currentYear), (0m, currentYearCompletePcn.Value));
            return result;
        }
        decimal complete = after == 0 ? 100m : currentYearCompletePcn.Value;
        decimal start = before == 0 ? 0m
            : currentYearStartPcn ?? TruncatedRatio(complete, before, before + 1);
        if (start > complete) throw inconsistentYearPlanData();

        if (before > 0)
        {
            decimal increment = TruncatedRatio(start, 1, before);
            decimal previous = 0m;
            for (long i = 0; i < before; i++)
            {
                decimal next = i == before - 1 ? start : previous + increment;
                result.Add(PlanId((long)startYear + i), (previous, next));
                previous = next;
            }
        }
        result.Add(PlanId(currentYear), (start, complete));
        if (after > 0)
        {
            // Subtraction stays exact even when a decimal intermediate would overflow.
            decimal increment = TruncatedDifferenceRatio(100m, complete, after);
            decimal previous = complete;
            for (long i = 1; i <= after; i++)
            {
                decimal next = i == after ? 100m : decimal.Truncate(previous + increment);
                if (next > 100m) next = 100m;
                result.Add(PlanId((long)currentYear + i), (previous, next));
                previous = next;
            }
        }
        return result;
    }

    private static long PlanId(long year) => checked(year * 100L + 1L);

    // Java divide(..., 0, RoundingMode.DOWN) truncates toward zero, not floor.
    // Integer rational arithmetic avoids decimal division rounding across an integer boundary.
    private static (BigInteger Coefficient, BigInteger Denominator) Parts(decimal value)
    {
        int[] bits = decimal.GetBits(value);
        var coefficient = (BigInteger)(uint)bits[0]
            | ((BigInteger)(uint)bits[1] << 32) | ((BigInteger)(uint)bits[2] << 64);
        if (bits[3] < 0) coefficient = -coefficient;
        return (coefficient, BigInteger.Pow(10, (bits[3] >> 16) & 0xff));
    }

    private static decimal TruncatedRatio(decimal value, long numerator, long denominator)
    {
        var p = Parts(value);
        return checked((decimal)(p.Coefficient * numerator / (p.Denominator * denominator)));
    }

    private static decimal TruncatedDifferenceRatio(decimal left, decimal right, long denominator)
    {
        var a = Parts(left);
        var b = Parts(right);
        return checked((decimal)((a.Coefficient * b.Denominator - b.Coefficient * a.Denominator)
            / (a.Denominator * b.Denominator * denominator)));
    }
}
