// Mechanical conversion from Java (model/entity/Measure.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.

namespace SemBackDotNet10.measureDomain.model.entity;

/// <summary>معادل Measure.java — getter/setter ها به property عمومی و annotation های JPA حذف شدند.</summary>
public class Measure : ProcessEntity<long?>
{
    public long? ReplannedStatus { get; set; }
    public bool ReplannedForHistory { get; set; }
    public override long? Id { get; set; }
    public string? TaskUrl { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? PerfectExtreme { get; set; }
    public decimal? WarnExtreme { get; set; }
    public decimal? WorstMin { get; set; }
    public decimal? CurrentYearTarget { get; set; }
    public decimal? LongTermTarget { get; set; }
    public string? TargetTheory { get; set; }
    public bool ZeroTarget { get; set; }
    public decimal? ShortTermTarget { get; set; }
    public bool FlgKpi { get; set; }
    public float? Weight { get; set; }
    public bool? DummyUpdate { get; set; }
    public bool ZeroInitial { get; set; }
    public long? Confirmstatus { get; set; }
    public MeasureCatalog MeasureCatalog { get; set; }
    public StrategyObject StrategyObject { get; set; }
    public PlanPeriod PlanPeriod { get; set; }
    public bool Deleted { get; set; }
    public decimal? OneYearLaterTarget { get; set; }
    public decimal? TwoYearLaterTarget { get; set; }
    public bool Planned { get; set; }
    public List<MeasureActPlan> MeasureActPlans { get; set; }
    public string? OutCome { get; set; }
    public decimal? BenchmarkValue { get; set; }
    public User Modir { get; set; }
    public User Tashilgar { get; set; }
    public bool AddToSupremeLevel { get; set; }
    public bool SendToParentOnly { get; set; }
}
