// Converted from: camundaDomain/controller/CamundaIntegrationController.java
// @RestController/@GetMapping -> [ApiController]/[HttpGet]؛
// @PreAuthorize("@semSecurityUtilityService.checkAccess(#headers.get('requesturi'))")
//   -> فیلتر دسترسی سفارشی (SecuredAccessAttribute) — پیاده‌سازی فیلتر در securityDomain انجام می‌شود؛
// @RequestHeader Map<String,String> headers -> IHttpContextAccessor.Request.Headers؛
// ResponseEntity<?> -> IActionResult.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.camundaDomain.service.interf;

namespace SemBackDotNet10.camundaDomain.controller;

[ApiController]
[Route("api/v1")]
public class CamundaIntegrationController : ControllerBase
{
    private readonly ICamundaService _camundaService;
    private readonly ICamundaIntegrationService _camundaIntegrationService;

    public CamundaIntegrationController(ICamundaService camundaService,
        ICamundaIntegrationService camundaIntegrationService)
    {
        _camundaService = camundaService;
        _camundaIntegrationService = camundaIntegrationService;
    }

    [HttpGet("fetchCamundaTask")]
    // [SecuredAccess] // معادل @PreAuthorize("@semSecurityUtilityService.checkAccess(#headers.get('requesturi'))")
    public IActionResult FetchCamundaTask([FromQuery] string taskId)
    {
        var response = _camundaService.FetchTaskVariablesFromTaskId(taskId);
        _camundaService.CheckAndManageClaim(taskId);
        var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        return new ContentResult
        {
            Content = body,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode,
        };
    }

    [HttpGet("unclaimtask")]
    // [SecuredAccess] // معادل @PreAuthorize("@semSecurityUtilityService.checkAccess(#headers.get('requesturi'))")
    public IActionResult UnClaimTask([FromQuery] string taskId)
    {
        var response = _camundaService.UnClaimTask(taskId);
        var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        return new ContentResult
        {
            Content = body,
            ContentType = "application/json",
            StatusCode = (int)response.StatusCode,
        };
    }
}
