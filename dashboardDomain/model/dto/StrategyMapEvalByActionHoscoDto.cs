namespace SemBackDotNet10.dashboardDomain.model.dto;

public class StrategyMapEvalByActionHoscoDto
{
    public string? DesLevel { get; set; }
    public int? StrategyMapId { get; set; }
    public int? PerspectId { get; set; }
    public int? ParentId { get; set; }
    public string? DesPerspectF { get; set; }
    public string? StrategyObjectName { get; set; }
    public decimal? Weight { get; set; }
    public decimal? WeightAverage { get; set; }
    public decimal? SumActionActualSelfObject { get; set; }
    public decimal? SumActionEvalSelfObject { get; set; }
}
