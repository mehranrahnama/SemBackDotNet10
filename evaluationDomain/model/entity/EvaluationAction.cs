using global::SemBackDotNet10.actionDomain.model.entity;
// Mechanical conversion from Java (model/entity/EvaluationAction.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.evaluationDomain.model.entity;

/// <summary>معادل EvaluationAction.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class EvaluationAction : ProcessEntity<long?>
{
    public override long? Id { get; set; }
    public EvaluationTeamHeaderLevel EvaluationTeamHeaderLevel { get; set; }
    public ActionActPlan ActionactPlan { get; set; }
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
