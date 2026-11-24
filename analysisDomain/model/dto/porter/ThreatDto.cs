namespace SemBackDotNet10.analysisDomain.model.dto.porter;

public class ThreatDto
{
    public long? Id { get; set; }
    public string? Name { get; set; }

    public ThreatDto()
    {
    }

    public ThreatDto(long? id, string? name)
    {
        Id = id;
        Name = name;
    }
}
