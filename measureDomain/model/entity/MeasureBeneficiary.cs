// Mechanical conversion from Java (model/entity/MeasureBeneficiary.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureBeneficiary.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureBeneficiary
{
    public long? Id { get; set; }
    public Measure Measure { get; set; }
    public PlaningLevelOrganization PlaningLevelOrganization { get; set; }
}
