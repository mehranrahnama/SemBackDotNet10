// Converted from: camundaDomain/model/dto/UpdateCamundaVariables.java

using SemBackDotNet10.camundaDomain.model.dto.@base;

namespace SemBackDotNet10.camundaDomain.model.dto;

public class UpdateCamundaVariables : ICamundaDto
{
    public Dictionary<string, TaskVariableItemDto>? Modifications { get; set; }
    public List<string>? Deletions { get; set; }

    public UpdateCamundaVariables() { }

    public UpdateCamundaVariables(Dictionary<string, TaskVariableItemDto>? modifications, List<string>? deletions)
    {
        Modifications = modifications;
        Deletions = deletions;
    }
}
