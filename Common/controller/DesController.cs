using Common.service.interf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Common.controller;

// The host must register this policy with the equivalent of checkAccess(requesturi).
// No permissive fallback: missing policy prevents requests from executing.
[ApiController]
[Route("api/v1/des")]
[Authorize(Policy = AccessPolicy)]
public sealed class DesController(IDesService service) : ControllerBase
{
    public const string AccessPolicy = "SemRequestUriAccess";
    [HttpGet("action-type")]
    public IActionResult GetAllActionTypes() => Ok(service.GetAllActionTypes());
    [HttpGet("action-priority")]
    public IActionResult GetAllActionPriorities() => Ok(service.GetAllActionPriorities());
    [HttpGet("measure-RepositoryStatus")]
    public IActionResult GetAllActionMeasureRepositoryStatus() => Ok(service.GetAllActionMeasureRepositoryStatus());
    [HttpGet("Certificate")]
    public IActionResult GetAllCertificate() => Ok(service.GetAllCertificate());
}
