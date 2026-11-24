using SemBackDotNet10.analysisDomain.model.entity.enums;

namespace SemBackDotNet10.analysisDomain.model.dto.porter;

public class PorterResultDto
{
    public long? Id { get; set; }
    public ProductDto? ProductDto { get; set; }
    public MarketDto? MarketDto { get; set; }
    public ThreatDto? ThreatDto { get; set; }
    public PorterGroupDto? PorterGroupDto { get; set; }
    public Threat? Result { get; set; }
    public string? Description { get; set; }
    public long? PlanId { get; set; }

    public PorterResultDto()
    {
    }

    public PorterResultDto(long? id, ProductDto? productDto, MarketDto? marketDto, ThreatDto? threatDto,
        PorterGroupDto? porterGroupDto, Threat? result, string? description, long? planId)
    {
        Id = id;
        ProductDto = productDto;
        MarketDto = marketDto;
        ThreatDto = threatDto;
        PorterGroupDto = porterGroupDto;
        Result = result;
        Description = description;
        PlanId = planId;
    }

    public PorterResultDto(long? id, long? planId, Threat? result, string? description)
    {
        Id = id;
        PlanId = planId;
        Result = result;
        Description = description;
    }
}
