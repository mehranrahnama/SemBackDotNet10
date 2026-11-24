namespace SemBackDotNet10.analysisDomain.model.dto.pestel;

public class PestelGroupDto
{
    public long? Id { get; set; }
    public string? Title { get; set; }
    public long? PlanId { get; set; }

    public PestelGroupDto()
    {
    }

    public PestelGroupDto(long? id, string? title, long? planId)
    {
        Id = id;
        Title = title;
        PlanId = planId;
    }

    public PestelGroupDto(long? id, string? title)
    {
        Id = id;
        Title = title;
    }
}
