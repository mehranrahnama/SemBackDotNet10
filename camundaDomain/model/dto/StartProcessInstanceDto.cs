// Converted from: camundaDomain/model/dto/StartProcessInstanceDto.java

using SemBackDotNet10.camundaDomain.model.dto.@base;

namespace SemBackDotNet10.camundaDomain.model.dto;

public class StartProcessInstanceDto : ICamundaDto
{
    public string? BusinessKey { get; set; }
    public string? CaseInstanceId { get; set; }
    public Dictionary<string, TaskVariableItemDto>? Variables { get; set; }
    public List<object>? StartInstructions { get; set; }
    public bool SkipCustomListeners { get; set; }
    public bool SkipToMappings { get; set; }
    public bool WithVariablesInReturn { get; set; }

    public StartProcessInstanceDto() { }

    public StartProcessInstanceDto(string? businessKey, string? caseInstanceId,
        Dictionary<string, TaskVariableItemDto>? variables, List<object>? startInstructions,
        bool skipCustomListeners, bool skipToMappings, bool withVariablesInReturn)
    {
        BusinessKey = businessKey;
        CaseInstanceId = caseInstanceId;
        Variables = variables;
        StartInstructions = startInstructions;
        SkipCustomListeners = skipCustomListeners;
        SkipToMappings = skipToMappings;
        WithVariablesInReturn = withVariablesInReturn;
    }
}
