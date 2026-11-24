// C# migration of utils/annotations/constraintsvalidation/NumericRangeExclusionValidator.java

namespace Common.utils.annotations.constraintsvalidation;

public static class NumericRangeExclusionValidator
{
 public static bool IsValid(long? value,long min,long max,IEnumerable<long> exclusions) => value is null || value>=min && value<=max && !exclusions.Contains(value.Value);
}
