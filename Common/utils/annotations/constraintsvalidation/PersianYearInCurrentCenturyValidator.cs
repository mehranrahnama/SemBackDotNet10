// C# migration of utils/annotations/constraintsvalidation/PersianYearInCurrentCenturyValidator.java

namespace Common.utils.annotations.constraintsvalidation;

public static class PersianYearInCurrentCenturyValidator
{
 public static bool IsValid(string? value,bool allowEmpty=false) {
 if(string.IsNullOrWhiteSpace(value))return allowEmpty;
 return value.Length>=4 && value[..4].All(c=>c>='0'&&c<='9') && int.TryParse(value[..4],out int year) && year>=1400 && year<=1499;
 }
}
