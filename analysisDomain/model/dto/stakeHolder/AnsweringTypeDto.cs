namespace SemBackDotNet10.analysisDomain.model.dto.stakeHolder;

public class AnsweringTypeDto
{
    public long? Id { get; set; }
    public string? Name { get; set; }

    public AnsweringTypeDto()
    {
    }

    public AnsweringTypeDto(long? id, string? name)
    {
        Id = id;
        Name = name;
    }
}
