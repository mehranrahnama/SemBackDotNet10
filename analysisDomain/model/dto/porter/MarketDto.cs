namespace SemBackDotNet10.analysisDomain.model.dto.porter;

public class MarketDto
{
    public long? Id { get; set; }
    public string? Title { get; set; }
    public long? PlanId { get; set; }

    public MarketDto()
    {
    }

    public MarketDto(long? id, string? title, long? planId)
    {
        Id = id;
        Title = title;
        PlanId = planId;
    }
}
