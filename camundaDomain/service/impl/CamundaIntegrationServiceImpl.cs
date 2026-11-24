using global::SemBackDotNet10.SecurityDomain.tokenprovider;
// Converted from: camundaDomain/service/impl/CamundaIntegrationServiceImpl.java
// RestTemplate -> HttpClient (sync با GetAwaiter().GetResult())؛
// RequestEntity/ResponseEntity -> HttpRequestMessage/HttpResponseMessage؛
// HttpHeaders -> HttpRequestMessage.Headers؛
// @Value("${camunda.base-path}") -> IConfiguration؛
// HttpServletRequest -> IHttpContextAccessor؛
// HttpClientErrorException.NotFound / HttpServerErrorException.InternalServerError
//   -> چون HttpClient پیش‌فرض روی کد وضعیت exception نمی‌پرد، وضعیت‌ها به‌صورت HttpResponseMessage.StatusCode بررسی می‌شوند
//      (این‌دقیقاً همان معنای catch در جاوا است).

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Common.exception;
using SemBackDotNet10.camundaDomain.model.dto;
using SemBackDotNet10.camundaDomain.model.dto.@base;
using SemBackDotNet10.camundaDomain.service.interf;
using SemBackDotNet10.camundaDomain.utils;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.camundaDomain.service.impl;

public class CamundaIntegrationServiceImpl : ICamundaIntegrationService
{
    private readonly string _camundaBasePath;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtTokenProvider _tokenProvider;
    private readonly HttpClient _httpClient;

    public CamundaIntegrationServiceImpl(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        JwtTokenProvider tokenProvider,
        HttpClient httpClient)
    {
        _camundaBasePath = configuration["ExternalServices:Camunda:BaseUrl"] ?? configuration["camunda:base-path"]
            ?? throw new InvalidOperationException("config 'camunda.base-path' is missing");
        _httpContextAccessor = httpContextAccessor;
        _tokenProvider = tokenProvider;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Claims a task for the authenticated user by assigning the task to the user based on the provided task ID.
    /// </summary>
    public HttpResponseMessage ClaimTask(ClaimTaskDto dto, string taskId)
    {
        HttpResponseMessage? response;
        try
        {
            var taskIdString = taskId.ToString();
            var headers = SetHttpHeaders(null);
            var uri = _camundaBasePath + CamundaAddresses.CHANGE_ASSIGNEE_BY_CLAIM;
            response = Fetch(dto, headers, HttpMethod.Post, string.Format(uri, taskIdString));
        }
        catch (HttpRequestException e) when ((e.StatusCode ?? HttpStatusCode.InternalServerError) == HttpStatusCode.InternalServerError)
        {
            throw new BusinessException("Task with given id does not exist or claiming was not successful.");
        }
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = response?.Content };
    }

    /// <summary>Marks a task as complete by updating its status in the Camunda engine.</summary>
    public HttpResponseMessage CompleteTask(CompleteTaskDto dto, string taskId)
    {
        var headers = SetHttpHeaders(null);
        var taskIdString = taskId.ToString();
        var uri = _camundaBasePath + CamundaAddresses.COMPLETE_TASK;
        var response = Fetch(dto, headers, HttpMethod.Post, string.Format(uri, taskIdString));
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            throw new BusinessException("task does not exist or the corresponding process instance could not be resumed successfully");
        }
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new BusinessException("The variable value or type is invalid");
        }
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = response.Content };
    }

    /// <summary>Removes the claim on a task, making it available for reassignment.</summary>
    public HttpResponseMessage UnClaimTask(string taskId)
    {
        var headers = SetHttpHeaders(null);
        var taskIdString = taskId.ToString();
        var uri = _camundaBasePath + CamundaAddresses.REMOVE_CLAIM_FROM_USER;
        var response = Fetch(null, headers, HttpMethod.Post, string.Format(uri, taskIdString));
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            throw new BusinessException("The Task with the given id does not exist.");
        }
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = response.Content };
    }

    /// <summary>Retrieves a paginated list of tasks from Camunda based on the provided filtering.</summary>
    // TODO در جاوا هم «Not Completed yet» بود — پارامتر header بلااستفاده ماند.
    public HttpResponseMessage GetTaskList(GetTaskListDto camundaDto)
    {
        var headers = SetHttpHeaders(null);
        var uri = _camundaBasePath + CamundaAddresses.GET_TASK_LIST + "?" + QueryUtils.BuildQueryParamFromClassFields(camundaDto);
        return Fetch(camundaDto, headers, HttpMethod.Post, string.Format(uri, camundaDto.ProcessDefinitionKey));
    }

    /// <summary>Creates a new task in the Camunda engine.</summary>
    public HttpResponseMessage CreateNewTask(CreateTaskDto dto)
    {
        var headers = SetHttpHeaders(null);
        var uri = _camundaBasePath + CamundaAddresses.CREATE_SINGLE_TASK;
        var response = Fetch(dto, headers, HttpMethod.Post, uri);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new BusinessException("not a valid delegationState is supplied");
        }
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = response.Content };
    }

    /// <summary>Retrieves the variables associated with a specific task; بدنه‌ی خالی «{}» معادل null است.</summary>
    public HttpResponseMessage? GetTaskVariables(string taskId)
    {
        var headers = SetHttpHeaders(null);
        var taskIdString = taskId.ToString();
        var uri = _camundaBasePath + CamundaAddresses.GET_TASK_VARIABLES;
        var response = Fetch(null, headers, HttpMethod.Get, string.Format(uri, taskIdString));
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            throw new BusinessException("Task id is null or does not exist.");
        }
        var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        if (body != null && body.Equals("{}"))
        {
            return null;
        }
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = response.Content };
    }

    /// <summary>Updates the variables associated with a task using the provided DTO.</summary>
    public HttpResponseMessage UpdateTaskVariables(UpdateTaskVariablesDto dto, string taskId)
    {
        var headers = SetHttpHeaders(null);
        var uri = _camundaBasePath + CamundaAddresses.UPDATE_TASK_VARIABLES;
        var response = Fetch(dto, headers, HttpMethod.Post, string.Format(uri, taskId));
        if (response.StatusCode == HttpStatusCode.BadRequest)
            throw new BusinessException("The variable value or type is invalid");
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            throw new BusinessException("Update or delete could not be executed because the task is null or does not exist.");
        }
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = response.Content };
    }

    public HttpResponseMessage UpdateProcessVariables(UpdateCamundaVariables dto, string taskId)
    {
        var headers = SetHttpHeaders(null);
        var uri = _camundaBasePath + CamundaAddresses.UPDATE_PROCESS_VARIABLES;
        var response = Fetch(dto, headers, HttpMethod.Post, string.Format(uri, taskId));
        if (response.StatusCode == HttpStatusCode.BadRequest)
            throw new BusinessException("The variable value or type is invalid");
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            throw new BusinessException("Update or delete could not be executed because the task is null or does not exist.");
        }
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = response.Content };
    }

    public HttpResponseMessage GetTask(string taskId)
    {
        var headers = SetHttpHeaders(null);
        var uri = _camundaBasePath + CamundaAddresses.GET_TASK_BY_ID;
        var response = Fetch(null, headers, HttpMethod.Get, string.Format(uri, taskId));
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new BusinessException("Task with id " + taskId + " does not exist");
        }
        return response;
    }

    public HttpResponseMessage UpdateTask(UpdateTaskDto dto, string taskId)
    {
        var headers = SetHttpHeaders(null);
        var uri = _camundaBasePath + CamundaAddresses.UPDATE_TASK_BY_ID;
        var response = Fetch(dto, headers, HttpMethod.Put, string.Format(uri, taskId));
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new BusinessException("Didn't supply a valid delegationState");
        }
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new BusinessException("Task with id " + taskId + " does not exist");
        }
        return response;
    }

    public HttpResponseMessage StartProcessInstance(StartProcessInstanceDto dto, string processKey)
    {
        var headers = SetHttpHeaders(null);
        var uri = _camundaBasePath + CamundaAddresses.START_PROCESS_INSTANCE;
        var response = Fetch(dto, headers, HttpMethod.Post, string.Format(uri, processKey));
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            throw new BusinessException("The instance could not be created successfully.");
        }
        else if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new BusinessException("\t\n" +
                "The instance could not be created due to an invalid variable value," +
                " for example if the value could not be parsed to an Integer value or" +
                " the passed variable type is not supported");
        }
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = response.Content };
    }

    public HttpResponseMessage DeleteProcessInstance(string processInstanceId)
    {
        var headers = SetHttpHeaders(null);
        var uri = _camundaBasePath + CamundaAddresses.DELETE_PROCESS_INSTANCE_BY_ID;
        var response = Fetch(null, headers, HttpMethod.Delete, string.Format(uri, processInstanceId));
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new BusinessException("فرایند با شناسه مورد نظر یافت نشد.");
        }
        return response;
    }

    /// <summary>
    /// creates a <see cref="string"/> of present time to set as a process's BusinessKey
    /// sample format: "20241215133021"
    /// </summary>
    public string GenerateNewBusinessKey()
    {
        var now = DateTime.Now;
        return now.ToString("yyyyMMddHHmmss");
    }

    public HttpResponseMessage GetProcessDefinitionByName(string processDefinitionName)
    {
        var headers = SetHttpHeaders(null);
        var uri = _camundaBasePath + CamundaAddresses.GET_PROCESS_DEFINITION_LIST_BY_NAME
                  + "?" + "name=" + Uri.EscapeDataString(processDefinitionName);
        var response = Fetch(null, headers, HttpMethod.Get, uri);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new BusinessException("برای مقادیر وارد شده نتیجه ای یافت نشد");
        }
        return response;
    }

    /// <summary>
    /// Builds the HTTP request message (معادل getRequestEntity جاوا).
    /// </summary>
    private HttpRequestMessage GetRequestEntity(ICamundaDto? camundaDto, HttpRequestHeaders headers, HttpMethod method, string uri)
    {
        try
        {
            var request = new HttpRequestMessage(method, uri);
            foreach (var h in headers) request.Headers.TryAddWithoutValidation(h.Key, h.Value);
            if (camundaDto != null)
            {
                var json = JsonSerializer.Serialize(camundaDto, camundaDto.GetType());
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            return request;
        }
        catch (InvalidOperationException e)
        {
            throw new InvalidOperationException("provided URI string is malformed or invalid", e);
        }
    }

    /// <summary>Sends an HTTP request and returns the response (معادل fetch جاوا — پاسخ به‌صورت رشته‌ی JSON).</summary>
    private HttpResponseMessage Fetch(ICamundaDto? camundaDto, HttpRequestHeaders headers, HttpMethod method, string uri)
    {
        var requestEntity = GetRequestEntity(camundaDto, headers, method, uri);
        var response = _httpClient.SendAsync(requestEntity).GetAwaiter().GetResult();
        var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var result = new HttpResponseMessage(response.StatusCode)
        {
            Content = new StringContent(body ?? string.Empty, Encoding.UTF8, "application/json")
        };
        return result;
    }

    /// <summary>
    /// Extracts the JWT token from the Authorization header of the current HTTP request.
    /// </summary>
    private string GetTokenFromRequest()
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new global::SemBackDotNet10.Common.exception.UnAuthorizedException();
        var authHeader = httpContext.Request.Headers["Authorization"].ToString();
        string token;
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            token = authHeader[7..];
        }
        else
        {
            throw new global::SemBackDotNet10.Common.exception.UnAuthorizedException();
        }
        // TODO REMOVE WHEN READY
        if (!_tokenProvider.CheckIsSsoToken(token))
        {
            throw new AccessForbiddenException("دسترسی به انجین کموندا ندارید.");
        }
        return token;
    }

    /// <summary>
    /// Builds request headers containing the Authorization header with the JWT token
    /// plus any custom headers; اگر customHeaders نال باشد فقط Authorization ست می‌شود.
    /// </summary>
    private HttpRequestHeaders SetHttpHeaders(Dictionary<string, string>? customHeaders)
    {
        var token = GetTokenFromRequest();
        var resultingHeader = new HttpRequestHeaders();
        resultingHeader["Authorization"] = "Bearer " + token;
        if (customHeaders != null)
        {
            foreach (var entry in customHeaders)
            {
                resultingHeader[entry.Key] = entry.Value;
            }
        }
        return resultingHeader;
    }
}

/// <summary>جایگزین سبک HttpHeaders درون‌خطی برای نگهداری هدرهای درخواست.</summary>
public class HttpRequestHeaders : Dictionary<string, string>
{
}
