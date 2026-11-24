namespace SemBackDotNet10.basicDomain.Model.Dto;

/// <summary>Base DTO shared by all request/response contracts.</summary>
public class BaseEntityDto
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}

// Domain DTOs ----------------------------------------------------------------



public class WeightsDto
{
    public long PlanId { get; set; }
    public long LevelId { get; set; }
    public List<double> Weights { get; set; } = new();
}
