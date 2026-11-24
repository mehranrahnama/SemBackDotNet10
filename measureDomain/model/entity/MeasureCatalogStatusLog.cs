// Mechanical conversion from Java (model/entity/MeasureCatalogStatusLog.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureCatalogStatusLog.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureCatalogStatusLog : AuditEntity<long?>
{
    public long? MeasureConfirmStatus { get; set; }
    public string? Hamesh { get; set; }
    public override long? Id { get; set; }
    public MeasureCatalog MeasureCatalog { get; set; }
}
