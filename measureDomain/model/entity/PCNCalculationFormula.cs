// Mechanical conversion from Java (model/entity/PCNCalculationFormula.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل PCNCalculationFormula.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class PCNCalculationFormula : AuditEntity<long?>
{
    public override long? Id { get; set; }
    public string? Conditions { get; set; }
    public string? Pcn_formula { get; set; }
    public Tenant Tenant { get; set; }
}
