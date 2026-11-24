namespace SemBackDotNet10.analysisDomain.model.dto.stakeHolder;

public class BehaviorStrategyDto
{
    public long? Id { get; set; }
    public string? Name { get; set; }

    public BehaviorStrategyDto()
    {
    }

    public BehaviorStrategyDto(long? id, string? name)
    {
        Id = id;
        Name = name;
    }
}
