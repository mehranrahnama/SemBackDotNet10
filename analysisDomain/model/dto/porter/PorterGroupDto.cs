namespace SemBackDotNet10.analysisDomain.model.dto.porter;

public class PorterGroupDto
{
    public long? Id { get; set; }
    public string? Title { get; set; }
    public long? PlanId { get; set; }

    public PorterGroupDto()
    {
    }

    public PorterGroupDto(long? id, string? title, long? planId)
    {
        Id = id;
        Title = title;
        PlanId = planId;
    }

    public PorterGroupDto(long? id, string? title)
    {
        Id = id;
        Title = title;
    }
}
