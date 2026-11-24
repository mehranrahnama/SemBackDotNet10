namespace SemBackDotNet10.analysisDomain.model.dto.porter;

public class PorterScoreDto
{
    public long? Id { get; set; }
    public PorterGroupDto? PorterGroupDto { get; set; }
    public long? PlanId { get; set; }
    public string? Title { get; set; }
    public int CurrentScore { get; set; }
    public int FutureScore { get; set; }
    public bool? Opportunity { get; set; }
    public MarketDto? MarketDto { get; set; }
    public ProductDto? ProductDto { get; set; }

    public PorterScoreDto()
    {
    }

    public PorterScoreDto(long? id, PorterGroupDto? porterGroupDto, long? planId, string? title, int currentScore,
        int futureScore, bool? opportunity, MarketDto? marketDto, ProductDto? productDto)
    {
        Id = id;
        PorterGroupDto = porterGroupDto;
        PlanId = planId;
        Title = title;
        CurrentScore = currentScore;
        FutureScore = futureScore;
        Opportunity = opportunity;
        MarketDto = marketDto;
        ProductDto = productDto;
    }

    public PorterScoreDto(long? id, long? porterGroupId, string? porterGroupTitle, long? planId, string? title,
        int currentScore, int futureScore, bool? opportunity)
    {
        Id = id;
        PorterGroupDto = new PorterGroupDto(porterGroupId, porterGroupTitle);
        PlanId = planId;
        Title = title;
        FutureScore = futureScore;
        CurrentScore = currentScore;
        Opportunity = opportunity;
    }
}
