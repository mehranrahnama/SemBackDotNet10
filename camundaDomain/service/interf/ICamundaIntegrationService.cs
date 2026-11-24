// Converted from: camundaDomain/service/interf/CamundaIntegrationService.java
// ResponseEntity<?> -> HttpResponseMessage؛ JSON با System.Text.Json.Nodes (JsonNode)

using System.Net.Http.Json;
using SemBackDotNet10.camundaDomain.model.dto;

namespace SemBackDotNet10.camundaDomain.service.interf;

/// <summary>معادل CamundaIntegrationService.java — لایه‌ی تماس مستقیم با REST API انجین کموندا.</summary>
public interface ICamundaIntegrationService
{
    HttpResponseMessage ClaimTask(ClaimTaskDto dto, string taskId);

    HttpResponseMessage CompleteTask(CompleteTaskDto dto, string taskId);

    HttpResponseMessage UnClaimTask(string taskId);

    HttpResponseMessage GetTaskList(GetTaskListDto camundaDto);

    HttpResponseMessage CreateNewTask(CreateTaskDto dto);

    /// <summary>متغیرهای یک task را برمی‌گرداند؛ اگر بدنه‌ی خالی («{}») باشد null برمی‌گردد.</summary>
    HttpResponseMessage? GetTaskVariables(string taskId);

    HttpResponseMessage UpdateTaskVariables(UpdateTaskVariablesDto dto, string taskId);

    HttpResponseMessage UpdateProcessVariables(UpdateCamundaVariables dto, string taskId);

    HttpResponseMessage GetTask(string taskId);

    HttpResponseMessage UpdateTask(UpdateTaskDto dto, string taskId);

    HttpResponseMessage StartProcessInstance(StartProcessInstanceDto dto, string processKey);

    HttpResponseMessage DeleteProcessInstance(string processInstanceId);

    string GenerateNewBusinessKey();

    HttpResponseMessage GetProcessDefinitionByName(string processDefinitionName);
}
