// C# migration of utils/annotations/constraints/NumericRangeExclusion.java

namespace Common.utils.annotations.constraints;

[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.Parameter)]
public sealed class NumericRangeExclusionAttribute(long min,long max,params long[] exclusions) : ValidationAttribute
{
 public override bool IsValid(object? value) => value is null || value is long number && constraintsvalidation.NumericRangeExclusionValidator.IsValid(number,min,max,exclusions);
}
