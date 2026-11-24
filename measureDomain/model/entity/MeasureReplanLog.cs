// Mechanical conversion from Java (model/entity/MeasureReplanLog.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureReplanLog.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureReplanLog : AuditEntity<long?>
{
    public override long? Id { get; set; }
    public int MeasurePeriodOld { get; set; }
    public int MeasurePeriodNew { get; set; }
    public int DesiredMethodOld { get; set; }
    public int DesiredMethodNew { get; set; }
    public int MeasurePayeshTypeOld { get; set; }
    public int MeasurePayeshTypeNew { get; set; }
    public decimal? CurrentYearTarget { get; set; }
    public Measure Measure { get; set; }
}
