namespace SemBackDotNet10.analysisDomain.model.dto.resource;

public class ResourceCompetencyRuleDto
{
    public long? Id { get; set; }
    public string? ValueV { get; set; }
    public string? ResultV { get; set; }
    public string? ValueR { get; set; }
    public string? ResultR { get; set; }
    public string? ValueI { get; set; }
    public string? ResultI { get; set; }
    public string? ValueO { get; set; }
    public string? ResultO { get; set; }
    public string? FinalResult { get; set; }
    public long? PlanId { get; set; }

    public ResourceCompetencyRuleDto()
    {
    }

    public ResourceCompetencyRuleDto(long? id, string? valueV, string? resultV, string? valueR, string? resultR,
        string? valueI, string? resultI, string? valueO, string? resultO, string? finalResult, long? planId)
    {
        Id = id;
        ValueV = valueV;
        ResultV = resultV;
        ValueR = valueR;
        ResultR = resultR;
        ValueI = valueI;
        ResultI = resultI;
        ValueO = valueO;
        ResultO = resultO;
        FinalResult = finalResult;
        PlanId = planId;
    }
}
