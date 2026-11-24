// C# migration of utils/annotations/constraints/PersianYearInCurrentCentury.java

namespace Common.utils.annotations.constraints;

[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.Parameter)]
public sealed class PersianYearInCurrentCenturyAttribute : ValidationAttribute
{
 public bool AllowEmpty { get; set; }
 public override bool IsValid(object? value) => (value is null || value is string) && constraintsvalidation.PersianYearInCurrentCenturyValidator.IsValid((string?)value,AllowEmpty);
}
