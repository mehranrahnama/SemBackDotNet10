namespace SemBackDotNet10.riskDomain.model.dto;

public class RiskDto
{
    public long? RiskId { get; set; }
    public string? DesRisk { get; set; }
    public string? Cause { get; set; }
    public string? Effect { get; set; }
    public long? Severity { get; set; }
    public long? Probability { get; set; }
    public RiskStrategyObjectDto? StrategyObjectDto { get; set; }
    public List<RiskActionDto> RiskActionDtoList { get; set; } = new();
    public List<RiskMeasureDto> RiskMeasureDtoList { get; set; } = new();
}

/// <summary>Minimal strategy-object reference (id, title, levelId) used by <see cref="RiskDto"/>.</summary>
public class RiskStrategyObjectDto
{
    public long? Id { get; set; }
    public string? Title { get; set; }
    public long? LevelId { get; set; }
}
