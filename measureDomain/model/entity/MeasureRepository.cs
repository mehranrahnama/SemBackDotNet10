// Mechanical conversion from Java (model/entity/MeasureRepository.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureRepository.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureRepository : AuditEntity<long?>
{
    public override long? Id { get; set; }
    public string? CodKpiEpm { get; set; }
    public string? MeasureName { get; set; }
    public int DesiredMethod { get; set; }
    public byte MeasureType { get; set; }
    public bool AddToSubLevels { get; set; }
    public bool Deleted { get; set; }
    public long? Confirmstatus { get; set; }
    public byte MeasurePayeshType { get; set; }
    public MeasureUnit MeasureUnit { get; set; }
    public string? Formula { get; set; }
    public string? FormulaDes { get; set; }
}
