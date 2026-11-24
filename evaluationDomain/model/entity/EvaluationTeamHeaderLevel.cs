// Mechanical conversion from Java (model/entity/EvaluationTeamHeaderLevel.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.evaluationDomain.model.entity;

/// <summary>معادل EvaluationTeamHeaderLevel.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class EvaluationTeamHeaderLevel
{
    public long? Id { get; set; }
    public EvaluationTeamHeader EvaluationTeamHeader { get; set; }
    public PlaningLevel PlaningLevel { get; set; }
}
