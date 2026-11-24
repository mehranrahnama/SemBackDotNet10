// Mechanical conversion from Java (model/entity/MeasureActPlan.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureActPlan.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureActPlan : ProcessEntity<long?>
{
    public DateTime? CorrectionStartDateAction { get; set; }
    public User CorrectionExecuter { get; set; }
    public override long? Id { get; set; }
    public string? TaskUrl { get; set; }
    public Measure Measure { get; set; }
    public long? Month { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? MonthPlanValue { get; set; }
    public decimal? ActualValue { get; set; }
    public bool Active { get; set; }
    public Byte Day { get; set; }
    public decimal? DailyValue { get; set; }
    public string? Deviation { get; set; }
    public string? Correction { get; set; }
    public long? Confirmstatus { get; set; }
    public PlanPeriod PlanPeriod { get; set; }
    public bool Deleted { get; set; }
    public DateTime? CorrectionDeadlineDate { get; set; }
    public decimal? CorrectionActualValue { get; set; }
    public string? OuCome { get; set; }
    public long? RoyalVisionActId { get; set; }
    public decimal? MeasurePCN { get; set; }
    public string? OutCome { get; set; }
    public User Excuter { get; set; }
}
