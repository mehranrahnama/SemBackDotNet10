// Converted from: camundaDomain/model/dto/CompleteTaskDto.java

using SemBackDotNet10.camundaDomain.model.dto.@base;

namespace SemBackDotNet10.camundaDomain.model.dto;

public class CompleteTaskDto : ICamundaDto
{
    public Dictionary<string, TaskVariableItemDto>? Variables { get; set; }
    public bool WithVariablesInReturn { get; set; }

    public CompleteTaskDto() { }

    public CompleteTaskDto(Dictionary<string, TaskVariableItemDto>? variables, bool withVariablesInReturn)
    {
        Variables = variables;
        WithVariablesInReturn = withVariablesInReturn;
    }
}
