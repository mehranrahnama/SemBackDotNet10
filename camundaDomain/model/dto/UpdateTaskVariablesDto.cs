// Converted from: camundaDomain/model/dto/UpdateTaskVariablesDto.java

using SemBackDotNet10.camundaDomain.model.dto.@base;

namespace SemBackDotNet10.camundaDomain.model.dto;

public class UpdateTaskVariablesDto : ICamundaDto
{
    public Dictionary<string, TaskVariableItemDto>? Modifications { get; set; }
    public List<string>? Deletions { get; set; }

    public UpdateTaskVariablesDto() { }

    public UpdateTaskVariablesDto(Dictionary<string, TaskVariableItemDto>? modifications, List<string>? deletions)
    {
        Modifications = modifications;
        Deletions = deletions;
    }
}
