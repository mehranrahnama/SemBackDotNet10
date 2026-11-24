using System.Text.Json.Serialization;

namespace SemBackDotNet10.dashboardDomain.model.dto;

/// <summary>نتیجه‌ی پروسیجر DashboardGetAllMeasureActPlanDataByUserIdPlanId (در جاوا @Entity بدون جدول).</summary>
public class DashboardDto
{
    public long? Id { get; set; }
    public string? MeasureName { get; set; }
    public string? DesLevel { get; set; }
    public long? UserId { get; set; }
    public long? MeasureId { get; set; }
    public decimal? WarnExtreme { get; set; }
    public decimal? PerfectExtreme { get; set; }
    public long? Month { get; set; }
    public decimal? PlanValue { get; set; }

    [JsonPropertyName("actualvalue")]
    public decimal? ActualValue { get; set; }

    [JsonPropertyName("pcn")]
    public decimal? PCN { get; set; }

    public long? PlanId { get; set; }
}
