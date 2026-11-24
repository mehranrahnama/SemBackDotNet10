namespace SemBackDotNet10.dashboardDomain.model.dto;

public class StrategyMapEvalByMeasureHoscoDto
{
    public string? DesLevel { get; set; }
    public int? StrategyMapId { get; set; }
    public int? PerspectId { get; set; }
    public int? ParentId { get; set; }
    public string? DesPerspectF { get; set; }
    public string? StrategyObjectName { get; set; }
    public decimal? Weight { get; set; }
    public decimal? WeightAverage { get; set; }
    public decimal? SumMeasureActualSelfObject { get; set; }
    public decimal? SumMeasureEvalSelfObject { get; set; }
}
