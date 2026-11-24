// Mechanical conversion from Java (model/entity/KeyMeasure.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل KeyMeasure.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class KeyMeasure : AuditEntity<long?>
{
    public override long? Id { get; set; }
    public MeasureCatalog MeasureCatalog { get; set; }
    public PlanPeriod PlanPeriod { get; set; }
    public decimal? TargetNumericValue { get; set; }
    public decimal? ActualNumericValue { get; set; }
    public string? TargetTextValue { get; set; }
    public string? ActualTextValue { get; set; }
}
