// Mechanical conversion from Java (model/entity/MeasureCatalogLog.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureCatalogLog.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureCatalogLog : AuditEntity<long?>
{
    public override long? Id { get; set; }
    public MeasureCatalog MeasureCatalog { get; set; }
    public User OldTashilgar { get; set; }
    public User NewTashilgar { get; set; }
    public User OldModir { get; set; }
    public User NewModir { get; set; }
}
