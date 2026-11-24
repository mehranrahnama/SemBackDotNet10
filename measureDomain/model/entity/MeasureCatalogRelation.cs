// Mechanical conversion from Java (model/entity/MeasureCatalogRelation.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureCatalogRelation.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureCatalogRelation : AuditEntity<long?>
{
    public override long? Id { get; set; }
    public PlanPeriod PlanPeriod { get; set; }
    public MeasureCatalog MainMeasureCatalog { get; set; }
    public MeasureCatalog RelatedMeasureCatalog { get; set; }
    public decimal? Weight { get; set; }
}
