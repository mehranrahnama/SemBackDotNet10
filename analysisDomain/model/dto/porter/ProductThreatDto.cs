using SemBackDotNet10.analysisDomain.model.entity.enums;

namespace SemBackDotNet10.analysisDomain.model.dto.porter;

public class ProductThreatDto
{
    public long? Id { get; set; }
    public ProductDto? ProductDto { get; set; }
    public MarketDto? MarketDto { get; set; }
    public ThreatDto? ThreatDto { get; set; }
    public PorterDto? PorterDto { get; set; }
    public Threat? Threat { get; set; }

    public ProductThreatDto()
    {
    }

    public ProductThreatDto(long? id, ProductDto? productDto, MarketDto? marketDto, ThreatDto? threatDto,
        PorterDto? porterDto, Threat? threat)
    {
        Id = id;
        ProductDto = productDto;
        MarketDto = marketDto;
        ThreatDto = threatDto;
        PorterDto = porterDto;
        Threat = threat;
    }

    public ProductThreatDto(long? id,
        long? productId, string? productTitle, long? productGroupId, string? productGroupTitle,
        long? marketId, string? marketTitle, long? marketPlanId)
    {
        Id = id;
        ProductDto = new ProductDto(productId, productTitle, productGroupId, productGroupTitle);
        MarketDto = new MarketDto(marketId, marketTitle, marketPlanId);
    }

    public ProductThreatDto(long? id,
        long? marketId, string? marketTitle, long? marketPlanId,
        long? productId, string? productTitle,
        long? productGroupId, string? productGroupTitle,
        long? porterId, string? porterTitle, string? noThreat, string? middleThreat, string? seriousThreat,
        Threat? threat)
    {
        Id = id;
        ProductDto = new ProductDto(productId, productTitle, productGroupId, productGroupTitle);
        MarketDto = new MarketDto(marketId, marketTitle, marketPlanId);
        PorterDto = new PorterDto(porterId, porterTitle, noThreat, middleThreat, seriousThreat);
        Threat = threat;
    }

    public ProductThreatDto(long? id,
        long? marketId, string? marketTitle, long? marketPlanId,
        long? productId, string? productTitle,
        long? productGroupId, string? productGroupTitle,
        long? porterId, string? porterTitle, string? noThreat, string? middleThreat, string? seriousThreat,
        long? threatId, string? threatName)
    {
        Id = id;
        ProductDto = new ProductDto(productId, productTitle, productGroupId, productGroupTitle);
        MarketDto = new MarketDto(marketId, marketTitle, marketPlanId);
        PorterDto = new PorterDto(porterId, porterTitle, noThreat, middleThreat, seriousThreat);
        ThreatDto = new ThreatDto(threatId, threatName);
    }
}
