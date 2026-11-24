using SemBackDotNet10.actionDomain.model.dto;

namespace SemBackDotNet10.riskDomain.model.dto;

public class RiskActionDto
{
    public long? Id { get; set; }
    public int RiskActionType { get; set; }
    public ActionPlanCatalogDto? ActionPlanCatalogDto { get; set; }
    public string? RiskActionTitle { get; set; }
    public long? RiskId { get; set; }
    public double? ProgressPercent { get; set; }
    public string? Description { get; set; }
}
