using SemBackDotNet10.measureDomain.model.dto;

namespace SemBackDotNet10.riskDomain.model.dto;

public class RiskMeasureDto
{
    public long? Id { get; set; }
    public MeasureCatalogDto? MeasureCatalogDto { get; set; }
    public string? RiskMeasureTitle { get; set; }
    public long? RiskId { get; set; }
}
