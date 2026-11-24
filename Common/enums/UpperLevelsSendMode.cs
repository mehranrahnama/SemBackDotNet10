// C# migration of enums/UpperLevelsSendMode.java

namespace Common.enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UpperLevelsSendMode { NONE, PARENT_ONLY, ALL }
public static class UpperLevelsSendModeFactory
{
 public static UpperLevelsSendMode Of(bool sendToUpperLevels, bool sendToParentOnly) {
  if (sendToUpperLevels && sendToParentOnly) throw new global::SemBackDotNet10.Common.exception.BusinessException("ارسال به همه‌ی کمیته‌های بالاتر و ارسال فقط به یک سطح بالاتر نمی‌توانند هم‌زمان انتخاب شوند.");
  return sendToParentOnly ? UpperLevelsSendMode.PARENT_ONLY : sendToUpperLevels ? UpperLevelsSendMode.ALL : UpperLevelsSendMode.NONE;
 }
}
