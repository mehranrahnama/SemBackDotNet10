// Mechanical conversion from Java (model/entity/MeasureActPlanCalc.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureActPlanCalc.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureActPlanCalc : AuditEntity<long?>
{
    public override long? Id { get; set; }
    public decimal? AchievedPCN { get; set; }
    public MeasureActPlan MeasureActPlan { get; set; }
}
