// Mechanical conversion from Java (model/entity/EvaluationTeam.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.evaluationDomain.model.entity;

/// <summary>معادل EvaluationTeam.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class EvaluationTeam
{
    public long? Id { get; set; }
    public EvaluationTeamHeader EvaluationTeamHeader { get; set; }
    public EvaluationTeamHeaderLevel EvaluationTeamHeaderLevel { get; set; }
    public User Rater { get; set; }
    public PlaningLevel PlaningLevel { get; set; }
}
