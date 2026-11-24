// Converted from: camundaDomain/service/interf/CamundaService.java
// ResponseEntity<?> -> HttpResponseMessage؛ JSON با System.Text.Json.Nodes (JsonNode)

using System.Net.Http;
using SemBackDotNet10.camundaDomain.model.dto;

namespace SemBackDotNet10.camundaDomain.service.interf;

/// <summary>معادل CamundaService.java — سرویس سطح بالای گردش‌کار (process/task) روی کموندا.</summary>
public interface ICamundaService
{
    HttpResponseMessage StartNewCamundaProcess(string caseInstanceId, string businessKey, string participantId,
        string definitionKey, List<string> assignee, string taskStatus);

    HttpResponseMessage StartNewCamundaProcess_v2(string caseInstanceId, string businessKey, string participantId,
        string definitionKey, Dictionary<string, List<string>> candidateGroups, string taskStatus);

    List<UpdateTaskDto> GetTaskFromCamundaProcess(GetTaskListDto taskDto, string taskDefinitionKey);

    HttpResponseMessage FetchTaskVariablesFromTaskId(string taskId);

    void CheckAndManageClaim(string taskId);

    void CheckAndSetClaim(string taskId, string userName);

    HttpResponseMessage UnClaimTask(string taskId);

    GetTaskListDto GetProcessInstanceVariables(HttpResponseMessage res);

    void UpdateTaskVariables(string taskId, string requester, string assignee, string owner,
        string requestNumber, string requestTitle, string semEntityId, string taskUrl);

    void UpdateTaskVariables_v2(Dictionary<string, string> variables);

    void UpdateProcessVariables(string processId, List<string> assignees);

    string GetProcessDefinitionKey(string processDefinitionName);

    void MoveProcessStep(string conditionKey, string conditionValue, string outCome, string businessKey,
        string processId, string definitionKey, string caseInstanceId);

    Dictionary<string, List<string>> SetCandidateGroups(string modir, string tashilgar, long planningLevelId);
}
