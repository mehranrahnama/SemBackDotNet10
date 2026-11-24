// Converted from: camundaDomain/model/dto/UpdateTaskDto.java
// توجه: برخلاف CreateTaskDto، فیلدهای priority و suspended در جاوا String هستند (عمداً حفظ شد).

using SemBackDotNet10.camundaDomain.model.dto.@base;

namespace SemBackDotNet10.camundaDomain.model.dto;

public class UpdateTaskDto : ICamundaDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Assignee { get; set; }
    public string? CandidateUsers { get; set; }
    public string? Owner { get; set; }
    public string? Created { get; set; }
    public string? LastUpdated { get; set; }
    public string? Due { get; set; }
    public string? FollowUp { get; set; }
    public string? DelegationState { get; set; }
    public string? Description { get; set; }
    public string? ExecutionId { get; set; }
    public string? ParentTaskId { get; set; }
    public string? Priority { get; set; }
    public string? ProcessDefinitionId { get; set; }
    public string? ProcessInstanceId { get; set; }
    public string? CaseExecutionId { get; set; }
    public string? CaseDefinitionId { get; set; }
    public string? CaseInstanceId { get; set; }
    public string? TaskDefinitionKey { get; set; }
    public string? Suspended { get; set; }
    public string? FormKey { get; set; }
    public string? TenantId { get; set; }
    public CamundaFormRefDto? CamundaFormRef { get; set; }
}
