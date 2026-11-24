// Converted from: camundaDomain/service/impl/CamundaServiceImpl.java
// Gson -> System.Text.Json؛ org.json JSONObject/JSONArray -> System.Text.Json.Nodes؛
// @Value("${sem.default.tenant.name}") -> IConfiguration؛
// SecurityUtils.getCurrentUser().get().getUserName() -> current authenticated principal name.
// GetTaskListDto.builder()...build() -> object initializer.

using System.Text.Json.Nodes;
using Common.exception;
using SemBackDotNet10.camundaDomain.model.dto;
using SemBackDotNet10.camundaDomain.service.interf;
using SemBackDotNet10.Common.exception;
using CurrentSecurity = global::SemBackDotNet10.SecurityDomain.Service.Impl.SecurityUtils;
using SemBackDotNet10.SecurityDomain.Service.Interf;

namespace SemBackDotNet10.camundaDomain.service.impl;

public class CamundaServiceImpl : ICamundaService
{
    private readonly ICamundaIntegrationService _camundaIntegrationService;
    private readonly IUserRoleService _userRoleService;
    private readonly string _tenantName;

    public CamundaServiceImpl(
        ICamundaIntegrationService camundaIntegrationService,
        IUserRoleService userRoleService,
        IConfiguration configuration)
    {
        _camundaIntegrationService = camundaIntegrationService;
        _userRoleService = userRoleService;
        _tenantName = configuration["Security:DefaultTenantName"] ?? configuration["sem:default:tenant:name"]
            ?? throw new InvalidOperationException("config 'sem.default.tenant.name' is missing");
    }

    public HttpResponseMessage StartNewCamundaProcess(string caseInstanceId, string businessKey, string participantId,
        string definitionKey, List<string> assignee, string taskStatus)
    {
        var newCamundaProcessInstance = new StartProcessInstanceDto();
        newCamundaProcessInstance.CaseInstanceId = caseInstanceId; // process id
        newCamundaProcessInstance.BusinessKey = businessKey; // business key
        newCamundaProcessInstance.WithVariablesInReturn = true;
        var userList = string.Join(",", assignee.Distinct()).ToLowerInvariant();
        var variables = new Dictionary<string, TaskVariableItemDto>();
        variables["tenant"] = new TaskVariableItemDto(_tenantName.ToUpperInvariant(), "String", null);
        variables["participantId"] = new TaskVariableItemDto(participantId ?? throw new InvalidOperationException("participantId is null"), "String", null);
        variables["definitionKey"] = new TaskVariableItemDto(definitionKey ?? throw new InvalidOperationException("definitionKey is null"), "String", null);
        variables["userList"] = new TaskVariableItemDto(userList, "String", null);
        variables["task_status"] = new TaskVariableItemDto(taskStatus, "String", null);
        newCamundaProcessInstance.Variables = variables;
        return _camundaIntegrationService.StartProcessInstance(newCamundaProcessInstance, definitionKey);
    }

    public HttpResponseMessage StartNewCamundaProcess_v2(string caseInstanceId, string businessKey, string participantId,
        string definitionKey, Dictionary<string, List<string>> candidateGroups, string taskStatus)
    {
        var newCamundaProcessInstance = new StartProcessInstanceDto();
        newCamundaProcessInstance.CaseInstanceId = caseInstanceId; // process id
        newCamundaProcessInstance.BusinessKey = businessKey; // business key
        newCamundaProcessInstance.WithVariablesInReturn = true;
        var variables = new Dictionary<string, TaskVariableItemDto>();
        variables["participantId"] = new TaskVariableItemDto(participantId ?? throw new InvalidOperationException("participantId is null"), "String", null);
        variables["definitionKey"] = new TaskVariableItemDto(definitionKey ?? throw new InvalidOperationException("definitionKey is null"), "String", null);
        variables["tenant"] = new TaskVariableItemDto(_tenantName.ToUpperInvariant(), "String", null);
        foreach (var (key, value) in candidateGroups)
        {
            variables[key] = new TaskVariableItemDto(string.Join(",", value), "String", null);
        }
        variables["task_status"] = new TaskVariableItemDto(taskStatus, "String", null);
        newCamundaProcessInstance.Variables = variables;
        return _camundaIntegrationService.StartProcessInstance(newCamundaProcessInstance, definitionKey);
    }

    public List<UpdateTaskDto> GetTaskFromCamundaProcess(GetTaskListDto taskDto, string taskDefinitionKey)
    {
        var taskResponse = _camundaIntegrationService.GetTaskList(taskDto);
        var taskBody = taskResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var taskArray = JsonNode.Parse(taskBody)?.AsArray()
            ?? throw new BusinessException("پاسخ کموندا آرایه نیست: " + taskBody);
        var taskList = new List<UpdateTaskDto>();
        foreach (var taskJson in taskArray)
        {
            if (taskJson == null) continue;
            taskList.Add(System.Text.Json.JsonSerializer.Deserialize<UpdateTaskDto>(taskJson.ToJsonString())
                ?? throw new BusinessException("task json could not be deserialized to UpdateTaskDto"));
        }
        return taskList;
    }

    public HttpResponseMessage FetchTaskVariablesFromTaskId(string taskId)
    {
        var res = _camundaIntegrationService.GetTaskVariables(taskId);
        if (res != null && res.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return res;
        }
        throw new BusinessException("Task Id is Null or does not exist!");
    }

    public void CheckAndManageClaim(string taskId)
    {
        var principal = CurrentSecurity.GetCurrentUser();
        var username = principal?.Identity?.IsAuthenticated == true ? principal.Identity.Name : null;
        if (username is null) throw new InvalidOperationException("Current user name is unavailable.");
        _camundaIntegrationService.ClaimTask(new ClaimTaskDto(username.ToLowerInvariant()), taskId);
    }

    public void CheckAndSetClaim(string taskId, string userName)
    {
        _camundaIntegrationService.ClaimTask(new ClaimTaskDto(userName.ToLowerInvariant()), taskId);
    }

    public HttpResponseMessage UnClaimTask(string taskId)
    {
        var response = _camundaIntegrationService.UnClaimTask(taskId);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new BusinessException("وظیفه مورد نظر یافت نشد.");
        }
        return response;
    }

    public GetTaskListDto GetProcessInstanceVariables(HttpResponseMessage res)
    {
        var body = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var obj = JsonNode.Parse(body) as JsonObject
            ?? throw new BusinessException("پاسخ کموندا شیء JSON نیست: " + body);
        var taskDto = new GetTaskListDto();
        var processId = obj.ContainsKey("id") ? obj["id"]!.GetValue<string>() : null;
        var caseInstanceId = obj.ContainsKey("caseInstanceId") ? obj["caseInstanceId"]!.GetValue<string>() : null;
        var businessKey = obj.ContainsKey("businessKey") ? obj["businessKey"]!.GetValue<string>() : null;
        var processVariables = obj.ContainsKey("variables") ? obj["variables"]!.AsObject() : null;
        if (processVariables == null)
        {
            // معادل assert processVariables != null در جاوا (در runtime با -ea غیرفعال بود، ولی اینجا صریح است)
            throw new InvalidOperationException("processVariables is null");
        }
        var definitionKey = processVariables.ContainsKey("definitionKey")
            ? processVariables["definitionKey"]!["value"]!.GetValue<string>() : null;
        taskDto.ProcessDefinitionKey = definitionKey;
        taskDto.ProcessInstanceId = processId;
        taskDto.Suspended = false;
        taskDto.CaseInstanceId = caseInstanceId;
        return taskDto;
    }

    public void UpdateTaskVariables(string taskId, string requester, string assignee, string owner,
        string requestNumber, string requestTitle, string semEntityId, string taskUrl)
    {
        var taskUrlVariable = new UpdateTaskVariablesDto();
        var variables = new Dictionary<string, TaskVariableItemDto>();
        variables["_requester"] = new TaskVariableItemDto(requester, "String", null);
        variables["tashilgar"] = new TaskVariableItemDto(assignee, "String", null);
        variables["modir"] = new TaskVariableItemDto(owner, "String", null);
        variables["_requestNumber"] = new TaskVariableItemDto(requestNumber, "String", null);
        variables["_requestTitle"] = new TaskVariableItemDto(requestTitle, "String", null);
        variables["measureCatalogId"] = new TaskVariableItemDto(semEntityId, "String", null);
        variables["taskUrl"] = new TaskVariableItemDto(taskUrl, "String", null);
        taskUrlVariable.Modifications = variables;
        _camundaIntegrationService.UpdateTaskVariables(taskUrlVariable, taskId);
    }

    /// <summary>
    /// Updates task variables for a given task using a map of variable names and values.
    /// کلیدهای مورد انتظار: _requester، tashilgar، modir، _requestNumber، _requestTitle،
    /// entityId/measureCatalogId، taskUrl و همچنین «taskId» که شناسه‌ی task است.
    /// </summary>
    public void UpdateTaskVariables_v2(Dictionary<string, string> variables)
    {
        var taskUrlVariable = new UpdateTaskVariablesDto();
        var persistingVariables = new Dictionary<string, TaskVariableItemDto>();
        foreach (var (key, value) in variables)
        {
            persistingVariables[key] = new TaskVariableItemDto(value, "String", null);
        }
        taskUrlVariable.Modifications = persistingVariables;
        _camundaIntegrationService.UpdateTaskVariables(taskUrlVariable, variables["taskId"]);
    }

    public void UpdateProcessVariables(string processId, List<string> assignees)
    {
        var dto = new UpdateCamundaVariables();
        var variables = new Dictionary<string, TaskVariableItemDto>();
        var userList = string.Join(",", assignees.Distinct()).ToLowerInvariant();
        variables["userList"] = new TaskVariableItemDto(userList, "String", null);
        dto.Modifications = variables;
        _camundaIntegrationService.UpdateProcessVariables(dto, processId);
    }

    public string GetProcessDefinitionKey(string processDefinitionName)
    {
        var res = _camundaIntegrationService.GetProcessDefinitionByName(processDefinitionName);
        var body = res.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            ?? throw new InvalidOperationException("response body is null");
        var processDefinitionList = JsonNode.Parse(body)?.AsArray()
            ?? throw new BusinessException("پاسخ کموندا آرایه نیست: " + body);
        if (processDefinitionList.Count == 0)
        {
            // معادل processDefinitionList.get(0) در جاوا که IndexOutOfBounds می‌داد
            throw new BusinessException("پاسخ خالی برای تعریف فرایند: " + processDefinitionName);
        }
        var processDefinition = JsonNode.Parse(processDefinitionList[0]!.ToJsonString()) as JsonObject
            ?? throw new BusinessException("تعریف فرایند شیء JSON نیست");
        // optString("key") -> اگر کلید نباشد رشته‌ی خالی برمی‌گردد
        return processDefinition.ContainsKey("key") ? processDefinition["key"]!.GetValue<string>() : string.Empty;
    }

    public void MoveProcessStep(string conditionKey, string conditionValue, string outCome, string businessKey,
        string processId, string definitionKey, string caseInstanceId)
    {
        var variables = new Dictionary<string, TaskVariableItemDto>();
        variables["outcome"] = new TaskVariableItemDto(outCome, "String", null);
        variables[conditionKey] = new TaskVariableItemDto(conditionValue, "String", null);
        var completeTaskDto = new CompleteTaskDto(variables, true);
        var task = GetTaskFromCamundaProcess(new GetTaskListDto
        {
            ProcessInstanceBusinessKey = businessKey,
            ProcessInstanceId = processId,
            CaseInstanceId = caseInstanceId,
        }, definitionKey);
        if (task.Count == 0)
        {
            // معادل task.getFirst() در جاوا که NoSuchElementException می‌داد
            throw new BusinessException("هیچ task ای برای businessKey=" + businessKey + " یافت نشد");
        }
        _camundaIntegrationService.CompleteTask(completeTaskDto, task[0].Id!);
    }

    public Dictionary<string, List<string>> SetCandidateGroups(string modir, string tashilgar, long planningLevelId)
    {
        var candidateGroups = new Dictionary<string, List<string>>();
        var adminUsers = _userRoleService.FindUsersByAdminRole()
            .Select(ur => ur.User!.UserName!.ToLowerInvariant()).ToList();
        var modirList = new List<string>(adminUsers.Count + 1);
        modirList.AddRange(adminUsers);
        modirList.Add(modir);
        var tashilgarList = new List<string>(adminUsers.Count + 1);
        tashilgarList.AddRange(adminUsers);
        tashilgarList.Add(tashilgar);
        var sanayeList = new List<string>(adminUsers);
        var taaliList = new List<string>(adminUsers.Count + 1);
        var dabirList = new List<string>(adminUsers.Count + 1);
        var raeesList = new List<string>(adminUsers.Count + 1);
        if (_tenantName.Equals("HOSCO", StringComparison.OrdinalIgnoreCase))
        {
            var sanayeUserRoles = _userRoleService.FindUsersBySanayeUserRoleByPlanningLevelId(planningLevelId)
                .Select(ur => ur.User!.UserName!.ToLowerInvariant()).ToList();
            sanayeList.AddRange(sanayeUserRoles);
            sanayeList.AddRange(adminUsers);
        }
        else if (_tenantName.Equals("IRISA", StringComparison.OrdinalIgnoreCase))
        {
            var taaliUserList = _userRoleService.FindUsersByTaaliUserRole(planningLevelId)
                .Select(ur => ur.User!.UserName!.ToLowerInvariant()).ToList();
            taaliList.AddRange(taaliUserList);
            taaliList.AddRange(adminUsers);
        }
        // TODO Add Dabir, Raees
        candidateGroups["taali_role_user_group"] = taaliList;
        candidateGroups["sanaye_role_user_group"] = sanayeList;
        candidateGroups["tashilgar_role_user_group"] = tashilgarList;
        candidateGroups["modir_role_user_group"] = modirList;
        // TODO FIX
        candidateGroups["raees_role_user_group"] = modirList;
        candidateGroups["dabir_role_user_group"] = modirList;
        return candidateGroups;
    }
}
