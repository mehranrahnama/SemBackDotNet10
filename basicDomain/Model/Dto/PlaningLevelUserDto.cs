namespace SemBackDotNet10.basicDomain.Model.Dto;

/// <summary>معادل PlaningLevelUserDto جاوا.</summary>
public class PlaningLevelUserDto
{
    public long? UserId { get; set; }

    public List<long> LevelIds { get; set; } = new();
}
