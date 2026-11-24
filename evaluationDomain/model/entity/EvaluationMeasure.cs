using global::SemBackDotNet10.measureDomain.model.entity;
// Mechanical conversion from Java (model/entity/EvaluationMeasure.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.evaluationDomain.model.entity;

/// <summary>معادل EvaluationMeasure.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class EvaluationMeasure : ProcessEntity<long?>
{
    public override long? Id { get; set; }
    public EvaluationTeamHeaderLevel EvaluationTeamHeaderLevel { get; set; }
    public MeasureActPlan MeasureActPlan { get; set; }
    public decimal? Assessment { get; set; }
    public decimal? AssessmentPCN { get; set; }
    public long? AssessmentType { get; set; }
    public string? RejectReason { get; set; }
    public long? ConfirmStatus { get; set; }
    public string? TaskUrl { get; set; }
    public string? Description { get; set; }
    public bool? Disabled { get; set; }
    public User RatedBy { get; set; }
}
