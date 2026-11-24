using SemBackDotNet10.basicDomain.Model.Dto;

namespace SemBackDotNet10.analysisDomain.model.dto.resource;

public class ResourceDto
{
    public long? Id { get; set; }
    public string? Title { get; set; }
    public bool? Resource { get; set; }
    public bool? ExistenceStatus { get; set; }
    public PlaningLevelDto? PlaningLevelDto { get; set; }
    public long? PlanId { get; set; }
    public long? Power { get; set; }
    public long? Importance { get; set; }
    public bool? Valuable { get; set; }
    public bool? Rare { get; set; }
    public bool? CostlyInitiate { get; set; }
    public bool? Organize { get; set; }
    public string? CompetencyResult { get; set; }

    public ResourceDto()
    {
    }

    public ResourceDto(long? id, string? title, bool? resource, bool? existenceStatus,
        PlaningLevelDto? planingLevelDto, long? planId, long? power, long? importance,
        bool? valuable, bool? rare, bool? costlyInitiate, bool? organize, string? competencyResult)
    {
        Id = id;
        Title = title;
        Resource = resource;
        ExistenceStatus = existenceStatus;
        PlaningLevelDto = planingLevelDto;
        PlanId = planId;
        Power = power;
        Importance = importance;
        Valuable = valuable;
        Rare = rare;
        CostlyInitiate = costlyInitiate;
        Organize = organize;
        CompetencyResult = competencyResult;
    }

    public ResourceDto(long? id, string? title, bool? resource, bool? existenceStatus,
        long? levelId, string? desLevel, long? planId, long? power, long? importance)
    {
        Id = id;
        Title = title;
        Resource = resource;
        ExistenceStatus = existenceStatus;
        PlaningLevelDto = new PlaningLevelDto { Id = levelId ?? 0, DesLevel = desLevel };
        PlanId = planId;
        Power = power;
        Importance = importance;
    }

    public ResourceDto(long? id, string? title, bool? resource, bool? existenceStatus,
        long? levelId, string? desLevel, long? planId, long? power, long? importance,
        bool? valuable, bool? rare, bool? costlyInitiate, bool? organize, string? competencyResult)
    {
        Id = id;
        Title = title;
        Resource = resource;
        ExistenceStatus = existenceStatus;
        PlaningLevelDto = new PlaningLevelDto { Id = levelId ?? 0, DesLevel = desLevel };
        PlanId = planId;
        Power = power;
        Importance = importance;
        Valuable = valuable;
        Rare = rare;
        CostlyInitiate = costlyInitiate;
        Organize = organize;
        CompetencyResult = competencyResult;
    }
}
