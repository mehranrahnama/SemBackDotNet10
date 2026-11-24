// Mechanical conversion from Java (model/entity/MeasureActPlanLog.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureActPlanLog.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureActPlanLog : AuditEntity<long?>
{
    public override long? Id { get; set; }
    public Measure Measure { get; set; }
    public long? Month { get; set; }
    public decimal? PlanValueNew { get; set; }
    public decimal? PlanValueOld { get; set; }
    public bool Deleted { get; set; }
    public decimal? ActualValueOld { get; set; }
    public decimal? ActualValueNew { get; set; }
}
