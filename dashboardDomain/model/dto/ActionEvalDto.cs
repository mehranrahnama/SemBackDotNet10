namespace SemBackDotNet10.dashboardDomain.model.dto;

public class ActionEvalDto
{
    // Request body
    public long? StrategyObjectId { get; set; }
    public long? PlanId { get; set; }
    public long? Month { get; set; }

    // Response
    public long? LevelId { get; set; }
    public long? OrganizationId { get; set; }
    public int? ActionDataByStrategyObjectId { get; set; }
    public string? DesLevel { get; set; }
    public string? ActionName { get; set; }
    public string? CurrentDate { get; set; }
    public decimal? Weight { get; set; }
    public decimal? AchievedPCNNormal { get; set; }
    public decimal? AchievedAssessmentPCN { get; set; }
    public decimal? AverageWeight { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal? Assessment { get; set; }
    public decimal? AssessmentPCN { get; set; }
}
