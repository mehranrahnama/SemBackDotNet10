// Mechanical conversion from Java (model/entity/EvaluationFeedBackQuestion.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.evaluationDomain.model.entity;

/// <summary>معادل EvaluationFeedBackQuestion.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class EvaluationFeedBackQuestion
{
    public long? Id { get; set; }
    public string? QuestionTitle { get; set; }
    public double? Weight { get; set; }
}
