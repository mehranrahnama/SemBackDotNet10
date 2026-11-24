#nullable enable

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class StrategyObjectTitleDto
{
    public long? Id { get; set; }
    public string? Title { get; set; }

    public StrategyObjectTitleDto() { }

    public StrategyObjectTitleDto(long? id, string? title)
    {
        Id = id;
        Title = title;
    }
}
