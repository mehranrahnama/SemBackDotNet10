// Mechanical conversion from Java (model/entity/MeasureCoworker.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureCoworker.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureCoworker
{
    public long? Id { get; set; }
    public Measure Measure { get; set; }
    public PlaningLevelOrganization PlaningLevelOrganization { get; set; }
}
