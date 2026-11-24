// Mechanical conversion from Java (model/entity/Process.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل Process.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class Process : AuditEntity<long?>
{
    public override long? Id { get; set; }
    public string? ProcessName { get; set; }
}
