// Mechanical conversion from Java (model/entity/EvaluationTeamHeader.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.evaluationDomain.model.entity;

/// <summary>معادل EvaluationTeamHeader.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class EvaluationTeamHeader : AuditEntity<long?>
{
    public override long? Id { get; set; }
    public PlanPeriod PlanPeriod { get; set; }
    public long? FromMonth { get; set; }
    public long? ToMonth { get; set; }
    public User MasterRater { get; set; }
    public List<EvaluationTeamHeaderLevel> EvaluationTeamHeaderLevelList { get; set; }
}
