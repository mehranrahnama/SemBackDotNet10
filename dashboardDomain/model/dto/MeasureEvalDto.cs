namespace SemBackDotNet10.dashboardDomain.model.dto;

public class MeasureEvalDto
{
    // Request body
    public long? StrategyObjectId { get; set; }
    public long? PlanId { get; set; }
    public long? LevelId { get; set; }
    public long? OrganizationId { get; set; }
    public long? Month { get; set; }

    // Response
    public int? MeasureDataByStrategyObjectId { get; set; }
    public string? MeasureName { get; set; }
    public string? DesLevel { get; set; }
    public string? CurrentDate { get; set; }
    public decimal? Weight { get; set; }
    public decimal? AchievedPCNNormalNew { get; set; }
    public decimal? AchievedAssessmentPCN { get; set; }
    public decimal? AverageWeight { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal? Assessment { get; set; }
    public decimal? AssessmentPCN { get; set; }
}
