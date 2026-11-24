namespace SemBackDotNet10.basicDomain.Model.Dto;

/// <summary>معادل MainStrategyDto جاوا.</summary>
public class MainStrategyDto
{
    public long Id { get; set; }

    public string? DesStrategyF { get; set; }

    public string? DateStart { get; set; }

    public string? DateEnd { get; set; }

    public long? PerspectiveId { get; set; }

    public string? PerspectiveDescription { get; set; }

    public long? LevelId { get; set; }

    public long? AccountableId { get; set; }

    public string? AccountableName { get; set; }
}
