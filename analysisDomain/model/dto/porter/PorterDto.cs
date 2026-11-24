namespace SemBackDotNet10.analysisDomain.model.dto.porter;

public class PorterDto
{
    public long? Id { get; set; }
    public PorterGroupDto? PorterGroupDto { get; set; }
    public string? Title { get; set; }
    public string? NoThreat { get; set; }
    public string? MiddleThreat { get; set; }
    public string? SeriousThreat { get; set; }

    public PorterDto()
    {
    }

    public PorterDto(long? id, PorterGroupDto? porterGroupDto, string? title, string? noThreat, string? middleThreat, string? seriousThreat)
    {
        Id = id;
        PorterGroupDto = porterGroupDto;
        Title = title;
        NoThreat = noThreat;
        MiddleThreat = middleThreat;
        SeriousThreat = seriousThreat;
    }

    public PorterDto(long? id, PorterGroupDto? porterGroupDto, string? title)
    {
        Id = id;
        PorterGroupDto = porterGroupDto;
        Title = title;
    }

    public PorterDto(long? id, string? title, string? noThreat, string? middleThreat, string? seriousThreat,
        long? porterGroupId, string? porterGroupTitle)
    {
        Id = id;
        Title = title;
        NoThreat = noThreat;
        MiddleThreat = middleThreat;
        SeriousThreat = seriousThreat;
        PorterGroupDto = new PorterGroupDto(porterGroupId, porterGroupTitle);
    }

    public PorterDto(long? id, string? title, long? porterGroupId, string? porterGroupTitle)
    {
        Id = id;
        Title = title;
        PorterGroupDto = new PorterGroupDto(porterGroupId, porterGroupTitle);
    }

    public PorterDto(long? id, string? title)
    {
        Id = id;
        Title = title;
    }

    public PorterDto(long? id, string? title, string? noThreat, string? middleThreat, string? seriousThreat)
    {
        Id = id;
        Title = title;
        NoThreat = noThreat;
        MiddleThreat = middleThreat;
        SeriousThreat = seriousThreat;
    }
}
