namespace SemBackDotNet10.webserviceDomain.domain.dto;

public class StrategyObjectWebServiceDto
{
    public StrategyObjectWebServiceDto()
    {
    }

    public StrategyObjectWebServiceDto(long? id, string? name)
    {
        Id = id;
        Name = name;
    }

    public long? Id { get; set; }

    public string? Name { get; set; }
}
