namespace SemBackDotNet10.analysisDomain.model.dto.porter;

public class ProductGroupDto
{
    public long? Id { get; set; }
    public string? Title { get; set; }
    public long? PlanId { get; set; }

    public ProductGroupDto()
    {
    }

    public ProductGroupDto(long? id, string? title, long? planId)
    {
        Id = id;
        Title = title;
        PlanId = planId;
    }

    public ProductGroupDto(long? id, string? title)
    {
        Id = id;
        Title = title;
    }
}
