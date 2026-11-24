// Mechanical conversion from Java (model/entity/EvaluationFeedBack.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.evaluationDomain.model.entity;

/// <summary>معادل EvaluationFeedBack.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class EvaluationFeedBack : AuditEntity<long?>
{
    public User RatedBy { get; set; }
    public override long? Id { get; set; }
    public EvaluationTeamHeaderLevel EvaluationTeamHeaderLevel { get; set; }
    public double? Weight { get; set; }
    public string? Question { get; set; }
    public EvaluationFeedBackQuestion EvaluationFeedBackQuestion { get; set; }
    public string? StrengthPoint { get; set; }
    public string? StrengthImprovement { get; set; }
    public decimal? Score { get; set; }
    public string? TaskUrl { get; set; }
    public long? ConfirmStatus { get; set; }
}
