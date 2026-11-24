// Mechanical conversion from Java (model/entity/MeasureCatalog.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل MeasureCatalog.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class MeasureCatalog : ProcessEntity<long?>
{
    public string? TaskUrl { get; set; }
    public override long? Id { get; set; }
    public decimal? WorstMin { get; set; }
    public string? TargetTheory { get; set; }
    public bool ZeroTarget { get; set; }
    public bool ZeroInitial { get; set; }
    public Byte DataType { get; set; }
    public int MeasurePeriod { get; set; }
    public int? ParentId { get; set; }
    public bool Deleted { get; set; }
    public long? Confirmstatus { get; set; }
    public decimal? MeasureMaxValue { get; set; }
    public int? MeasureStatus { get; set; }
    public int? OriginId { get; set; }
    public PlaningLevel PlaningLevel { get; set; }
    public MeasureRepository MeasureRepository { get; set; }
    public User Modir { get; set; }
    public User Tashilgar { get; set; }
    public User TashilgarHamkar { get; set; }
    public bool AddToSupremLevel { get; set; }
    public string? OutCome { get; set; }
    public string? Code { get; set; }
    public string? CaseInstanceId { get; set; }
    public string? BusinessKey { get; set; }
    public string? ProcessId { get; set; }
    public string? DefinitionId { get; set; }
}
