// Converted from: camundaDomain/model/dto/GetTaskListDto.java
// lombok @Builder -> در دات‌نت از object initializer استفاده شود:
//   مثال: new GetTaskListDto { ProcessInstanceBusinessKey = ..., ProcessInstanceId = ..., CaseInstanceId = ... }

using SemBackDotNet10.camundaDomain.model.dto.@base;

namespace SemBackDotNet10.camundaDomain.model.dto;

public class GetTaskListDto : ICamundaDto
{
    public List<string>? TaskIdIn { get; set; }
    public string? TaskId { get; set; }
    public string? Assignee { get; set; }
    public List<string>? AssigneeIn { get; set; }
    public string? ProcessInstanceBusinessKey { get; set; }
    public string? ProcessInstanceId { get; set; }
    public string? ProcessDefinitionId { get; set; }
    public string? ProcessDefinitionKey { get; set; }
    public string? ExecutionId { get; set; }
    public bool Suspended { get; set; }
    public string? CaseInstanceId { get; set; }
    public List<Dictionary<string, string>>? OrQueries { get; set; }
}
